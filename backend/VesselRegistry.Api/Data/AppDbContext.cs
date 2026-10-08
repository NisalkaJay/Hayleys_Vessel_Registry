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
        }
    }
}