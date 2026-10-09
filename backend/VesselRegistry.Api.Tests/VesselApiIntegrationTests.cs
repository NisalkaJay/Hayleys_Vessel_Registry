using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using VesselRegistry.Api.Dtos;

namespace VesselRegistry.Api.Tests;

[Collection(SqlServerIntegrationTestCollection.Name)]
public sealed class VesselApiIntegrationTests
{
    private readonly SqlServerIntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    public VesselApiIntegrationTests(SqlServerIntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-User-Id", "1");
        _client.DefaultRequestHeaders.Add("X-Company-Id", "1");
    }

    [Fact]
    public async Task VesselTypes_UsesRealSqlServerAndReturnsSeededTypes()
    {
        var response = await _client.GetAsync("/api/vessel-types");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(5, body.GetProperty("data").GetArrayLength());
    }

    [Fact]
    public async Task VesselList_UsesRealSqlServerAndReturnsTenantData()
    {
        var response = await _client.GetAsync("/api/vessels?page=1&pageSize=10&sortBy=vesselName&sortDirection=asc");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.Equal(10, body.GetProperty("data").GetProperty("items").GetArrayLength());
        Assert.True(body.GetProperty("data").GetProperty("totalCount").GetInt32() >= 13);
    }

    [Fact]
    public async Task CrossCompanyVessel_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/vessels/2");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("NotFound", body.GetProperty("errorCode").GetString());
    }

    [Fact]
    public async Task MissingTenantHeader_ReturnsValidationError()
    {
        using var client = _fixture.Factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/vessel-types");
        request.Headers.Add("X-User-Id", "1");

        var response = await client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Validation", body.GetProperty("errorCode").GetString());
    }

    [Fact]
    public async Task DuplicateImo_ReturnsConflictFromRealUniqueIndex()
    {
        var request = new VesselDto
        {
            VesselName = "Duplicate IMO",
            ImoNumber = "1234567",
            VesselTypeId = 1,
            FlagCountry = "Sri Lanka",
            GrossTonnage = 1000,
            YearBuilt = 2020,
            IsActive = true
        };

        var response = await _client.PostAsJsonAsync("/api/vessels", request);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("Duplicate", body.GetProperty("errorCode").GetString());
    }
}
