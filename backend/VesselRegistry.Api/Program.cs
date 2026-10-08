using Microsoft.EntityFrameworkCore;
using VesselRegistry.Api.Data;
using VesselRegistry.Api.Entities;
using VesselRegistry.Api.Middleware;
using VesselRegistry.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
        {
            // Disable default automatic 400 responses to use our custom ApiResponse envelope
            options.SuppressModelStateInvalidFilter = true;
        });


builder.Services.AddScoped<IVesselTypeService, VesselTypeService>();
builder.Services.AddScoped<IVesselService, VesselService>();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Allow CORS from localhost:4200[cite: 2]
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build(); 

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    if (!await dbContext.VesselTypes.AnyAsync())
    {
        dbContext.VesselTypes.AddRange(
            new VesselType { VesselTypeId = 1, Name = "Bulk Carrier" },
            new VesselType { VesselTypeId = 2, Name = "Container" },
            new VesselType { VesselTypeId = 3, Name = "Tanker" },
            new VesselType { VesselTypeId = 4, Name = "General Cargo" },
            new VesselType { VesselTypeId = 5, Name = "Ro-Ro" });
        await dbContext.SaveChangesAsync();
    }

    if (!await dbContext.Vessels.AnyAsync())
    {
        dbContext.Vessels.AddRange(
            new Vessel
            {
                VesselId = 1,
                CompanyId = 1,
                VesselName = "MV Ocean Star",
                ImoNumber = "1234567",
                VesselTypeId = 1,
                FlagCountry = "Sri Lanka",
                GrossTonnage = 12500.00m,
                YearBuilt = 2018,
                IsActive = true,
                CreatedBy = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Vessel
            {
                VesselId = 2,
                CompanyId = 2,
                VesselName = "MV Pacific Trader",
                ImoNumber = "7654321",
                VesselTypeId = 2,
                FlagCountry = "Singapore",
                GrossTonnage = 24800.00m,
                YearBuilt = 2020,
                IsActive = true,
                CreatedBy = 1,
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            });
        await dbContext.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
// 1. Catch all unexpected errors first[cite: 4]
app.UseMiddleware<ExceptionHandlingMiddleware>(); 

app.UseHttpsRedirection();

// 2. Apply CORS policy before authorization and endpoints[cite: 2]
app.UseCors("AllowAngularApp"); 

// 3. Validate X-User-Id and X-Company-Id headers[cite: 2]
app.UseMiddleware<HeaderValidationMiddleware>();

app.UseAuthorization();

// 4. Map the API endpoints last
app.MapControllers();

app.Run();