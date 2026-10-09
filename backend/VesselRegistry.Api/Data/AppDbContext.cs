using Microsoft.EntityFrameworkCore;
using VesselRegistry.Api.Entities;

namespace VesselRegistry.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<VesselType> VesselTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure unique VesselType Name[cite: 2]
            modelBuilder.Entity<VesselType>()
                .HasIndex(vt => vt.Name)
                .IsUnique();

            // Ensure IMO Number is unique within a single company[cite: 3]
            modelBuilder.Entity<Vessel>()
                .HasIndex(v => new { v.CompanyId, v.ImoNumber })
                .IsUnique();

            // Seed lookup data[cite: 2]
            modelBuilder.Entity<VesselType>().HasData(
                new VesselType { VesselTypeId = 1, Name = "Bulk Carrier" },
                new VesselType { VesselTypeId = 2, Name = "Container" },
                new VesselType { VesselTypeId = 3, Name = "Tanker" },
                new VesselType { VesselTypeId = 4, Name = "General Cargo" },
                new VesselType { VesselTypeId = 5, Name = "Ro-Ro" }
            );

            modelBuilder.Entity<Vessel>().HasData(
                new Vessel
                {
                    VesselId = 1, CompanyId = 1, VesselName = "MV Ocean Star", ImoNumber = "1234567",
                    VesselTypeId = 1, FlagCountry = "Sri Lanka", GrossTonnage = 12500.00m,
                    YearBuilt = 2018, IsActive = true, CreatedBy = 1,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Vessel
                {
                    VesselId = 2, CompanyId = 2, VesselName = "MV Pacific Trader", ImoNumber = "7654321",
                    VesselTypeId = 2, FlagCountry = "Singapore", GrossTonnage = 24800.00m,
                    YearBuilt = 2020, IsActive = true, CreatedBy = 1,
                    CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc)
                },
                new Vessel { VesselId = 3, CompanyId = 1, VesselName = "MV Southern Cross", ImoNumber = "1000003", VesselTypeId = 3, FlagCountry = "Australia", GrossTonnage = 9800.00m, YearBuilt = 2017, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 4, CompanyId = 2, VesselName = "MV Eastern Wind", ImoNumber = "1000004", VesselTypeId = 4, FlagCountry = "Japan", GrossTonnage = 11200.00m, YearBuilt = 2016, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 4, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 5, CompanyId = 1, VesselName = "MV Coral Bay", ImoNumber = "1000005", VesselTypeId = 5, FlagCountry = "Malta", GrossTonnage = 7600.00m, YearBuilt = 2015, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 6, CompanyId = 2, VesselName = "MV Blue Horizon", ImoNumber = "1000006", VesselTypeId = 1, FlagCountry = "Greece", GrossTonnage = 15300.00m, YearBuilt = 2019, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 7, CompanyId = 1, VesselName = "MV Island Queen", ImoNumber = "1000007", VesselTypeId = 2, FlagCountry = "Cyprus", GrossTonnage = 8900.00m, YearBuilt = 2014, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 7, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 8, CompanyId = 2, VesselName = "MV Golden Wave", ImoNumber = "1000008", VesselTypeId = 3, FlagCountry = "Norway", GrossTonnage = 18700.00m, YearBuilt = 2018, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 9, CompanyId = 1, VesselName = "MV Silver Dawn", ImoNumber = "1000009", VesselTypeId = 4, FlagCountry = "Panama", GrossTonnage = 10400.00m, YearBuilt = 2013, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 10, CompanyId = 2, VesselName = "MV Northern Light", ImoNumber = "1000010", VesselTypeId = 5, FlagCountry = "Liberia", GrossTonnage = 13200.00m, YearBuilt = 2021, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 11, CompanyId = 1, VesselName = "MV Emerald Sea", ImoNumber = "1000011", VesselTypeId = 1, FlagCountry = "Bahamas", GrossTonnage = 14500.00m, YearBuilt = 2012, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 12, CompanyId = 2, VesselName = "MV Atlantic Pride", ImoNumber = "1000012", VesselTypeId = 2, FlagCountry = "United Kingdom", GrossTonnage = 22100.00m, YearBuilt = 2017, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 13, CompanyId = 1, VesselName = "MV Harbor Star", ImoNumber = "1000013", VesselTypeId = 3, FlagCountry = "India", GrossTonnage = 6700.00m, YearBuilt = 2011, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 13, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 14, CompanyId = 2, VesselName = "MV Ocean Venture", ImoNumber = "1000014", VesselTypeId = 4, FlagCountry = "Denmark", GrossTonnage = 11900.00m, YearBuilt = 2016, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 14, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 15, CompanyId = 1, VesselName = "MV Sea Falcon", ImoNumber = "1000015", VesselTypeId = 5, FlagCountry = "Turkey", GrossTonnage = 9200.00m, YearBuilt = 2019, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 16, CompanyId = 2, VesselName = "MV Coastal Runner", ImoNumber = "1000016", VesselTypeId = 1, FlagCountry = "Italy", GrossTonnage = 10800.00m, YearBuilt = 2015, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 17, CompanyId = 1, VesselName = "MV Liberty Bell", ImoNumber = "1000017", VesselTypeId = 2, FlagCountry = "United States", GrossTonnage = 17600.00m, YearBuilt = 2020, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 17, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 18, CompanyId = 2, VesselName = "MV Rainforest", ImoNumber = "1000018", VesselTypeId = 3, FlagCountry = "Brazil", GrossTonnage = 8400.00m, YearBuilt = 2014, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 19, CompanyId = 1, VesselName = "MV Star Voyager", ImoNumber = "1000019", VesselTypeId = 4, FlagCountry = "Canada", GrossTonnage = 12600.00m, YearBuilt = 2018, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 19, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 20, CompanyId = 2, VesselName = "MV Trade Wind", ImoNumber = "1000020", VesselTypeId = 5, FlagCountry = "France", GrossTonnage = 9900.00m, YearBuilt = 2013, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 21, CompanyId = 1, VesselName = "MV Lighthouse", ImoNumber = "1000021", VesselTypeId = 1, FlagCountry = "Portugal", GrossTonnage = 7300.00m, YearBuilt = 2010, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 21, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 22, CompanyId = 2, VesselName = "MV Sea Explorer", ImoNumber = "1000022", VesselTypeId = 2, FlagCountry = "Germany", GrossTonnage = 16400.00m, YearBuilt = 2021, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 22, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 23, CompanyId = 1, VesselName = "MV Royal Palm", ImoNumber = "1000023", VesselTypeId = 3, FlagCountry = "Indonesia", GrossTonnage = 13800.00m, YearBuilt = 2016, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 24, CompanyId = 2, VesselName = "MV Cargo Master", ImoNumber = "1000024", VesselTypeId = 4, FlagCountry = "South Korea", GrossTonnage = 20100.00m, YearBuilt = 2019, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc) },
                new Vessel { VesselId = 25, CompanyId = 1, VesselName = "MV Ocean Pearl", ImoNumber = "1000025", VesselTypeId = 5, FlagCountry = "New Zealand", GrossTonnage = 8100.00m, YearBuilt = 2012, IsActive = true, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 25, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}