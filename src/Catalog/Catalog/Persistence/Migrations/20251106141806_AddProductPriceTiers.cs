using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductPriceTiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductPriceTiers",
                columns: table => new
                {
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductPriceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FromQuantity = table.Column<int>(type: "int", nullable: false),
                    ToQuantity = table.Column<int>(type: "int", nullable: true),
                    TierType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset?>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<DateTimeOffset?>(type: "datetimeoffset", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPriceTiers", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_ProductPriceTiers_ProductPrices_OrganizationId_ProductPriceId",
                        columns: x => new { x.OrganizationId, x.ProductPriceId },
                        principalTable: "ProductPrices",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceTiers_IsDeleted",
                table: "ProductPriceTiers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceTiers_OrganizationId",
                table: "ProductPriceTiers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceTiers_OrganizationId_ProductPriceId",
                table: "ProductPriceTiers",
                columns: new[] { "OrganizationId", "ProductPriceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceTiers_TenantId",
                table: "ProductPriceTiers",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPriceTiers");
        }
    }
}
