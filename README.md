# Hayleys Vessel Registry

Vessel Registry is a tenant-aware ASP.NET Core Web API with a standalone Angular frontend.

## Project layout

```text
backend/VesselRegistry.Api/
  Controllers/
  Data/
  Dtos/
  Entities/
  Middleware/
  Migrations/
  Services/

frontend/
  src/app/core/
  src/app/shared/
  src/app/features/vessels/
```

## Prerequisites

The application was developed and verified with:

- .NET SDK `10.0.401`
- .NET target framework `net10.0`
- Node.js `22.17.0`
- npm `10.9.2`
- Angular CLI `21.0.0`
- Angular `21.0.0`
- Microsoft SQL Server 2025 (RTM) `17.0.1000.7` (X64)
- SQL Server Enterprise Developer Edition (64-bit) on Windows 10 Pro
- SQL Server instance on `localhost`, using Windows authentication

## Database configuration

The development connection string is in:

```text
backend/VesselRegistry.Api/appsettings.Development.json
```

The default connection uses Windows authentication:

```text
Server=localhost;Database=VesselRegistryDb;Trusted_Connection=True;TrustServerCertificate=True;
```

Do not commit passwords or other production credentials. Use user secrets or environment-specific configuration for real deployments.

To use a different SQL Server instance, edit `ConnectionStrings:DefaultConnection`
in `appsettings.Development.json`, or provide the value through user secrets or
another environment-specific configuration source.

## Create the database and load seed data

The repository contains two EF Core migrations:

- `20261008150335_InitialCreate`
- `20261009052441_SeedVessels`

From the repository root, restore/build the backend and apply the migrations:

```powershell
dotnet restore ".\backend\VesselRegistry.Api\VesselRegistry.Api.csproj"
dotnet ef database update `
  --project ".\backend\VesselRegistry.Api\VesselRegistry.Api.csproj" `
  --startup-project ".\backend\VesselRegistry.Api\VesselRegistry.Api.csproj"
```

If `dotnet ef` is not installed, install the global EF tool once:

```powershell
dotnet tool install --global dotnet-ef
```

The API also runs `Database.MigrateAsync()` during startup. The migrations
create both tables, add the constraints and indexes, and load five vessel
types plus 25 vessels split between companies 1 and 2. The startup seed
fallback repairs an empty development database after migrations have run.

For a disposable development reset, stop the API and run:

```powershell
dotnet ef database drop `
  --project ".\backend\VesselRegistry.Api\VesselRegistry.Api.csproj" `
  --startup-project ".\backend\VesselRegistry.Api\VesselRegistry.Api.csproj" `
  --force
dotnet ef database update `
  --project ".\backend\VesselRegistry.Api\VesselRegistry.Api.csproj" `
  --startup-project ".\backend\VesselRegistry.Api\VesselRegistry.Api.csproj"
```

## Run the backend

From the repository root:

```powershell
dotnet run `
  --project ".\backend\VesselRegistry.Api\VesselRegistry.Api.csproj" `
  --launch-profile http
```

The API listens on:

```text
http://localhost:5031
```

At startup, pending EF Core migrations are applied. The migrations create:

- `VesselTypes`, seeded with Bulk Carrier, Container, Tanker, General Cargo, and Ro-Ro.
- `Vessels`, seeded with 25 vessels split across companies 1 and 2.

If the development database has an inconsistent migration history, stop the API and recreate the development database before restarting it.

## Run the frontend

Open a second terminal:

```powershell
Set-Location ".\frontend"
npm install
npm start
```

Open:

```text
http://localhost:4200/vessels
```

The frontend API URL and tenant identity are configured in:

```text
frontend/src/environments/environment.ts
```

The Angular HTTP interceptors automatically add:

```text
X-User-Id
X-Company-Id
```

## API endpoints

All requests require positive `X-User-Id` and `X-Company-Id` headers.

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/vessel-types` | Get vessel type lookup values |
| GET | `/api/vessels` | Get a filtered, server-paged vessel list |
| GET | `/api/vessels/{id}` | Get one vessel owned by the caller's company |
| POST | `/api/vessels` | Create a vessel |
| PUT | `/api/vessels/{id}` | Update a vessel owned by the caller's company |
| DELETE | `/api/vessels/{id}` | Deactivate a vessel |

Example request:

```powershell
$headers = @{
    "X-User-Id" = "1"
    "X-Company-Id" = "1"
}

Invoke-RestMethod `
    -Uri "http://localhost:5031/api/vessels?page=1&pageSize=10" `
    -Headers $headers
```

Every response uses the envelope:

```json
{
  "success": true,
  "data": {},
  "errorCode": null,
  "message": null
}
```

Cross-company single-vessel reads, updates, and deletes return `404 NotFound`.
Duplicate IMO numbers within one company return `409 Duplicate`.
Invalid request data returns `400 Validation` with field errors.

## Run endpoint tests

Start the backend first, then run:

```powershell
.\backend\VesselRegistry.Api\test-endpoints.ps1
```

The script covers headers, lookup data, filtering, pagination, validation, tenant isolation, create, update, duplicate IMO handling, deactivation, and reactivation-related list behavior.

The endpoint test script creates and modifies development data. Reset the
development database first if repeatable seed-only results are required.

## Completed requirements

- ASP.NET Core Web API with thin controllers and service interfaces.
- EF Core SQL Server code-first schema, migrations, unique constraints, and seed data.
- Tenant header middleware requiring positive `X-User-Id` and `X-Company-Id` values.
- Company isolation for vessel reads, updates, and deactivation, with cross-company resources returning 404.
- DTO-only API contracts and a consistent response envelope.
- Server-side search, filtering, counting, and pagination.
- Validation errors, invalid vessel-type errors, duplicate IMO handling, and generic exception handling.
- Standalone Angular application with lazy-loaded vessel routes.
- Signal-based list/form state and typed reactive forms.
- Shared form-field, form-select, and data-table components.
- Tenant, envelope, and API-error HTTP interceptors.
- Create, edit, deactivate, reactivate, not-found, loading, empty, and error states.
- Vessel list search, filters, and pagination are preserved in the URL for refreshes and shareable views.
- Optional enhancements: xUnit service tests, Swagger UI at `/swagger`, Dapper-backed list paging, server-side sortable columns, and a read-only vessel detail page.

## Build validation

Backend:

```powershell
dotnet build ".\backend\VesselRegistry.Api\VesselRegistry.Api.csproj"
```

SQL Server integration tests:

```powershell
dotnet test ".\backend\VesselRegistry.Api.Tests\VesselRegistry.Api.Tests.csproj"
```

The integration-test fixture uses the existing local SQL Server instance but
creates a separate `VesselRegistry_IntegrationTests_<processId>` database. It applies the
real EF Core migrations, runs HTTP requests through the real application
pipeline, verifies Dapper queries and SQL Server constraints, and drops only
that dedicated test database during cleanup. It never uses the normal
`VesselRegistryDb` development database.

Frontend:

```powershell
Set-Location ".\frontend"
npm run build
```

## Skipped work

- Automated Angular unit and end-to-end tests were not added because the
  requested scope prioritized the working screens and API integration.
- Production deployment, authentication/authorization, and secret management
  were not added; the project uses the required development tenant headers.

## Assumptions

- `localhost` resolves to the SQL Server instance used by the developer.
- Windows authentication is available, or the connection string will be
  replaced with credentials appropriate for the local environment.
- Development tenant identity is represented by the values in
  `frontend/src/environments/environment.ts`.
- Deactivation is a soft delete. Reactivation uses the existing vessel update
  endpoint with `isActive: true`; the DELETE endpoint remains deactivation-only.
- The database can be recreated during development when migration history is
  inconsistent. Production databases should be backed up and migrated using
  an approved deployment process.
- Dapper is used only for the filtered vessel list query; EF Core remains the
  source of truth for migrations, writes, and detail reads.

## Approximate implementation time

The implementation, troubleshooting, endpoint verification, frontend work,
and documentation took approximately three working days.
