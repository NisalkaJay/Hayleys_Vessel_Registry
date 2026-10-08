using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VesselRegistry.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedVessels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Vessels",
                columns: new[] { "VesselId", "CompanyId", "VesselName", "ImoNumber", "VesselTypeId", "FlagCountry", "GrossTonnage", "YearBuilt", "IsActive", "CreatedBy", "CreatedAt", "ModifiedBy", "ModifiedAt" },
                values: new object[,]
                {
                    { 1, 1, "MV Ocean Star", "1234567", 1, "Sri Lanka", 12500.00m, 2018, true, 1, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), null, null },
                    { 2, 2, "MV Pacific Trader", "7654321", 2, "Singapore", 24800.00m, 2020, true, 1, new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Vessels", keyColumn: "VesselId", keyValue: 1);
            migrationBuilder.DeleteData(table: "Vessels", keyColumn: "VesselId", keyValue: 2);
        }
    }
}
