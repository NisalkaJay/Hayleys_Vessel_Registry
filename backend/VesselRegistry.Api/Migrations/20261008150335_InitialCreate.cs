using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VesselRegistry.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VesselTypes",
                columns: table => new
                {
                    VesselTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselTypes", x => x.VesselTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Vessels",
                columns: table => new
                {
                    VesselId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    VesselName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImoNumber = table.Column<string>(type: "char(7)", maxLength: 7, nullable: false),
                    VesselTypeId = table.Column<int>(type: "int", nullable: false),
                    FlagCountry = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    GrossTonnage = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    YearBuilt = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vessels", x => x.VesselId);
                    table.ForeignKey(
                        name: "FK_Vessels_VesselTypes_VesselTypeId",
                        column: x => x.VesselTypeId,
                        principalTable: "VesselTypes",
                        principalColumn: "VesselTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "VesselTypes",
                columns: new[] { "VesselTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Bulk Carrier" },
                    { 2, "Container" },
                    { 3, "Tanker" },
                    { 4, "General Cargo" },
                    { 5, "Ro-Ro" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_CompanyId_ImoNumber",
                table: "Vessels",
                columns: new[] { "CompanyId", "ImoNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_VesselTypeId",
                table: "Vessels",
                column: "VesselTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselTypes_Name",
                table: "VesselTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vessels");

            migrationBuilder.DropTable(
                name: "VesselTypes");
        }
    }
}
