using Microsoft.EntityFrameworkCore;
using VesselRegistry.Api.Data;
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