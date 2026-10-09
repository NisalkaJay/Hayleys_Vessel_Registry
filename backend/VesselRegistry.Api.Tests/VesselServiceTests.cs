using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VesselRegistry.Api.Data;
using VesselRegistry.Api.Dtos;
using VesselRegistry.Api.Entities;
using VesselRegistry.Api.Services;

namespace VesselRegistry.Api.Tests;

public class VesselServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsNotFoundForAnotherCompany()
    {
        await using var context = CreateContext();
        context.VesselTypes.Add(new VesselType { VesselTypeId = 1, Name = "Bulk Carrier" });
        context.Vessels.Add(CreateVessel(1, 2));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var result = await service.GetByIdAsync(1, 1);

        Assert.False(result.Success);
        Assert.Equal("NotFound", result.ErrorCode);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateImoWithinCompany()
    {
        await using var context = CreateContext();
        context.VesselTypes.Add(new VesselType { VesselTypeId = 1, Name = "Bulk Carrier" });
        context.Vessels.Add(CreateVessel(1, 1));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var result = await service.CreateAsync(new VesselDto
        {
            VesselName = "Duplicate",
            ImoNumber = "1234567",
            VesselTypeId = 1,
            FlagCountry = "Sri Lanka",
            GrossTonnage = 1000,
            YearBuilt = 2020,
            IsActive = true
        }, 1, 9);

        Assert.False(result.Success);
        Assert.Equal("Duplicate", result.ErrorCode);
    }

    [Fact]
    public async Task DeactivateAsync_SetsInactiveForOwnedVessel()
    {
        await using var context = CreateContext();
        context.VesselTypes.Add(new VesselType { VesselTypeId = 1, Name = "Bulk Carrier" });
        context.Vessels.Add(CreateVessel(1, 1));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var result = await service.DeactivateAsync(1, 1, 7);

        Assert.True(result.Success);
        Assert.False((await context.Vessels.FindAsync(1))!.IsActive);
    }

    private static VesselService CreateService(AppDbContext context)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=(local);Database=Test;Trusted_Connection=True;"
            })
            .Build();
        return new VesselService(context, configuration);
    }

    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Vessel CreateVessel(int id, int companyId) => new()
    {
        VesselId = id,
        CompanyId = companyId,
        VesselName = "MV Test",
        ImoNumber = "1234567",
        VesselTypeId = 1,
        FlagCountry = "Sri Lanka",
        GrossTonnage = 1000,
        YearBuilt = 2020,
        CreatedBy = 1,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };
}
