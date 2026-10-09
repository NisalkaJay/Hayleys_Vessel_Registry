using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                columns: new[] { "VesselId", "CompanyId", "CreatedAt", "CreatedBy", "FlagCountry", "GrossTonnage", "ImoNumber", "IsActive", "ModifiedAt", "ModifiedBy", "VesselName", "VesselTypeId", "YearBuilt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Sri Lanka", 12500.00m, "1234567", true, null, null, "MV Ocean Star", 1, 2018 },
                    { 2, 2, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Singapore", 24800.00m, "7654321", true, null, null, "MV Pacific Trader", 2, 2020 },
                    { 3, 1, new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Australia", 9800.00m, "1000003", true, null, null, "MV Southern Cross", 3, 2017 },
                    { 4, 2, new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Japan", 11200.00m, "1000004", true, null, null, "MV Eastern Wind", 4, 2016 },
                    { 5, 1, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Malta", 7600.00m, "1000005", true, null, null, "MV Coral Bay", 5, 2015 },
                    { 6, 2, new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Greece", 15300.00m, "1000006", true, null, null, "MV Blue Horizon", 1, 2019 },
                    { 7, 1, new DateTime(2026, 1, 7, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Cyprus", 8900.00m, "1000007", true, null, null, "MV Island Queen", 2, 2014 },
                    { 8, 2, new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Norway", 18700.00m, "1000008", true, null, null, "MV Golden Wave", 3, 2018 },
                    { 9, 1, new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Panama", 10400.00m, "1000009", true, null, null, "MV Silver Dawn", 4, 2013 },
                    { 10, 2, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Liberia", 13200.00m, "1000010", true, null, null, "MV Northern Light", 5, 2021 },
                    { 11, 1, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Bahamas", 14500.00m, "1000011", true, null, null, "MV Emerald Sea", 1, 2012 },
                    { 12, 2, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), 1, "United Kingdom", 22100.00m, "1000012", true, null, null, "MV Atlantic Pride", 2, 2017 },
                    { 13, 1, new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Utc), 1, "India", 6700.00m, "1000013", true, null, null, "MV Harbor Star", 3, 2011 },
                    { 14, 2, new DateTime(2026, 1, 14, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Denmark", 11900.00m, "1000014", true, null, null, "MV Ocean Venture", 4, 2016 },
                    { 15, 1, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Turkey", 9200.00m, "1000015", true, null, null, "MV Sea Falcon", 5, 2019 },
                    { 16, 2, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Italy", 10800.00m, "1000016", true, null, null, "MV Coastal Runner", 1, 2015 },
                    { 17, 1, new DateTime(2026, 1, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "United States", 17600.00m, "1000017", true, null, null, "MV Liberty Bell", 2, 2020 },
                    { 18, 2, new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Brazil", 8400.00m, "1000018", true, null, null, "MV Rainforest", 3, 2014 },
                    { 19, 1, new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Canada", 12600.00m, "1000019", true, null, null, "MV Star Voyager", 4, 2018 },
                    { 20, 2, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), 1, "France", 9900.00m, "1000020", true, null, null, "MV Trade Wind", 5, 2013 },
                    { 21, 1, new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Portugal", 7300.00m, "1000021", true, null, null, "MV Lighthouse", 1, 2010 },
                    { 22, 2, new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Germany", 16400.00m, "1000022", true, null, null, "MV Sea Explorer", 2, 2021 },
                    { 23, 1, new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Indonesia", 13800.00m, "1000023", true, null, null, "MV Royal Palm", 3, 2016 },
                    { 24, 2, new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Utc), 1, "South Korea", 20100.00m, "1000024", true, null, null, "MV Cargo Master", 4, 2019 },
                    { 25, 1, new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), 1, "New Zealand", 8100.00m, "1000025", true, null, null, "MV Ocean Pearl", 5, 2012 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Vessels",
                keyColumn: "VesselId",
                keyValue: 25);
        }
    }
}
