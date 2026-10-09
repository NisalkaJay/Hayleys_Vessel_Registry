using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VesselRegistry.Api.Data;

namespace VesselRegistry.Api.Tests;

public sealed class SqlServerIntegrationTestFixture : IAsyncLifetime
{
    public static readonly string DatabaseName = $"VesselRegistry_IntegrationTests_{Environment.ProcessId}";
    private const string ServerConnectionString =
        "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=10;";

    public WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await EnsureDatabaseDoesNotExistAsync();
        await CreateDatabaseAsync();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<AppDbContext>();
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(BuildDatabaseConnectionString()));
                });
            });
    }

    public async Task DisposeAsync()
    {
        if (Factory is not null)
        {
            await Factory.DisposeAsync();
        }
        await DropDatabaseAsync();
    }

    public static string BuildDatabaseConnectionString() =>
        $"Server=localhost;Database={DatabaseName};Trusted_Connection=True;TrustServerCertificate=True;";

    private static async Task CreateDatabaseAsync()
    {
        await using var connection = new SqlConnection(ServerConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE [{DatabaseName}]";
        await command.ExecuteNonQueryAsync();
    }

    private static async Task EnsureDatabaseDoesNotExistAsync()
    {
        await using var connection = new SqlConnection(ServerConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            IF DB_ID(N'{DatabaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{DatabaseName}];
            END
            """;
        await command.ExecuteNonQueryAsync();
    }

    private static async Task DropDatabaseAsync()
    {
        await using var connection = new SqlConnection(ServerConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            IF DB_ID(N'{DatabaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{DatabaseName}];
            END
            """;
        await command.ExecuteNonQueryAsync();
    }
}

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SqlServerIntegrationTestCollection : ICollectionFixture<SqlServerIntegrationTestFixture>
{
    public const string Name = "SQL Server integration tests";
}
