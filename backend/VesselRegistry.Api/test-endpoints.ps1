param(
    [string]$BaseUrl = "http://localhost:5031"
)

$ErrorActionPreference = "Stop"
$passed = 0
$failed = 0

function Invoke-TestRequest {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Path,
        [int]$ExpectedStatus,
        [hashtable]$Headers = @{},
        [object]$Body = $null
    )

    $uri = "$BaseUrl$Path"
    $requestBody = $null
    if ($null -ne $Body) {
        $requestBody = $Body | ConvertTo-Json -Depth 10
    }

    try {
        $response = Invoke-WebRequest `
            -Uri $uri `
            -Method $Method `
            -Headers $Headers `
            -ContentType "application/json" `
            -Body $requestBody `
            -UseBasicParsing
        $status = [int]$response.StatusCode
        $content = $response.Content
    }
    catch {
        if ($null -eq $_.Exception.Response) {
            throw
        }

        $status = [int]$_.Exception.Response.StatusCode
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $content = $reader.ReadToEnd()
        $reader.Dispose()
    }

    if ($status -eq $ExpectedStatus) {
        $script:passed++
        Write-Host "PASS [$status] $Method $Path - $Name" -ForegroundColor Green
    }
    else {
        $script:failed++
        Write-Host "FAIL [$status, expected $ExpectedStatus] $Method $Path - $Name" -ForegroundColor Red
        Write-Host $content
    }

    return @{
        StatusCode = $status
        Content = $content
    }
}

function Get-JsonData {
    param([hashtable]$Response)

    if ([string]::IsNullOrWhiteSpace($Response.Content)) {
        return $null
    }

    return ($Response.Content | ConvertFrom-Json).data
}

$company1 = @{ "X-User-Id" = "1"; "X-Company-Id" = "1" }
$company2 = @{ "X-User-Id" = "2"; "X-Company-Id" = "2" }
$invalidHeaders = @{ "X-User-Id" = "abc"; "X-Company-Id" = "0" }
$createdVesselId = $null

Write-Host "Testing $BaseUrl" -ForegroundColor Cyan
Write-Host ""

# Middleware and lookup endpoint
Invoke-TestRequest "Missing tenant headers" "GET" "/api/vessel-types" 400
Invoke-TestRequest "Invalid tenant headers" "GET" "/api/vessel-types" 400 $invalidHeaders
Invoke-TestRequest "Get vessel types" "GET" "/api/vessel-types" 200 $company1

# Vessel list, filtering, and pagination
Invoke-TestRequest "List vessels" "GET" "/api/vessels" 200 $company1
Invoke-TestRequest "Search vessels" "GET" "/api/vessels?search=Ocean" 200 $company1
Invoke-TestRequest "Filter by vessel type" "GET" "/api/vessels?vesselTypeId=1" 200 $company1
Invoke-TestRequest "Filter active vessels" "GET" "/api/vessels?isActive=true" 200 $company1
Invoke-TestRequest "Valid pagination" "GET" "/api/vessels?page=1&pageSize=1" 200 $company1
Invoke-TestRequest "Page zero rejected" "GET" "/api/vessels?page=0" 400 $company1
Invoke-TestRequest "Negative page size rejected" "GET" "/api/vessels?pageSize=-1" 400 $company1
Invoke-TestRequest "Page size over 100 rejected" "GET" "/api/vessels?pageSize=101" 400 $company1

# Get and tenant isolation
Invoke-TestRequest "Get vessel owned by company 1" "GET" "/api/vessels/1" 200 $company1
Invoke-TestRequest "Get vessel from another company returns 404" "GET" "/api/vessels/2" 404 $company1
Invoke-TestRequest "Get missing vessel returns 404" "GET" "/api/vessels/999999" 404 $company1

# Validation failures
$invalidVessel = @{
    VesselName = ""
    ImoNumber = "123"
    VesselTypeId = 0
    FlagCountry = ""
    GrossTonnage = 0
    YearBuilt = (Get-Date).Year + 1
    IsActive = $true
}
Invoke-TestRequest "Invalid vessel body rejected" "POST" "/api/vessels" 400 $company1 $invalidVessel

$invalidTypeVessel = @{
    VesselName = "Invalid Type Vessel"
    ImoNumber = "1111111"
    VesselTypeId = 999999
    FlagCountry = "Sri Lanka"
    GrossTonnage = 1000
    YearBuilt = 2020
    IsActive = $true
}
Invoke-TestRequest "Unknown vessel type rejected" "POST" "/api/vessels" 400 $company1 $invalidTypeVessel

# Create and duplicate IMO handling
$newVessel = @{
    VesselName = "Terminal Test Vessel"
    ImoNumber = "2468135"
    VesselTypeId = 1
    FlagCountry = "Sri Lanka"
    GrossTonnage = 1500.50
    YearBuilt = 2022
    IsActive = $true
}
$createResponse = Invoke-TestRequest "Create vessel" "POST" "/api/vessels" 201 $company1 $newVessel
$createdData = Get-JsonData $createResponse
if ($null -ne $createdData) {
    $createdVesselId = $createdData.vesselId
}

Invoke-TestRequest "Duplicate IMO rejected" "POST" "/api/vessels" 409 $company1 $newVessel

# Update and update isolation
$updatedVessel = @{
    VesselName = "Terminal Test Vessel Updated"
    ImoNumber = "2468135"
    VesselTypeId = 2
    FlagCountry = "Singapore"
    GrossTonnage = 1600.75
    YearBuilt = 2023
    IsActive = $true
}

if ($null -ne $createdVesselId) {
    Invoke-TestRequest "Update owned vessel" "PUT" "/api/vessels/$createdVesselId" 200 $company1 $updatedVessel
    Invoke-TestRequest "Update vessel with duplicate IMO rejected" "PUT" "/api/vessels/$createdVesselId" 409 $company1 @{
        VesselName = "Terminal Test Vessel Updated"
        ImoNumber = "1234567"
        VesselTypeId = 1
        FlagCountry = "Sri Lanka"
        GrossTonnage = 1600.75
        YearBuilt = 2023
        IsActive = $true
    }
    Invoke-TestRequest "Update missing vessel returns 404" "PUT" "/api/vessels/999999" 404 $company1 $updatedVessel
    Invoke-TestRequest "Deactivate owned vessel" "DELETE" "/api/vessels/$createdVesselId" 200 $company1
    Invoke-TestRequest "List inactive vessels" "GET" "/api/vessels?isActive=false" 200 $company1
    Invoke-TestRequest "Deactivate already inactive vessel" "DELETE" "/api/vessels/$createdVesselId" 200 $company1
}

Invoke-TestRequest "Update another company's vessel returns 404" "PUT" "/api/vessels/2" 404 $company1 $updatedVessel
Invoke-TestRequest "Delete another company's vessel returns 404" "DELETE" "/api/vessels/2" 404 $company1

Write-Host ""
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor $(if ($failed -eq 0) { "Green" } else { "Red" })

if ($failed -gt 0) {
    exit 1
}

exit 0
