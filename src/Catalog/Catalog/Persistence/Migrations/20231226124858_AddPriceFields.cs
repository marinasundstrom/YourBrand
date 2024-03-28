using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "Products",
                newName: "Sku");

            migrationBuilder.RenameColumn(
                name: "GTIN",
                table: "Products",
                newName: "Gtin");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyDisplayOptions_RoundingDecimals",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PricingOptions_ProfitMarginPercentage",
                table: "Stores",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountRate",
                table: "Products",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Vat",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "VatRate",
                table: "Products",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyDisplayOptions_RoundingDecimals",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PricingOptions_ProfitMarginPercentage",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "Products",
                newName: "SKU");

            migrationBuilder.RenameColumn(
                name: "Gtin",
                table: "Products",
                newName: "GTIN");
        }
    }
}
