using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations;

/// <inheritdoc />
public partial class UpdateProduct : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Vat",
            table: "Products");

        migrationBuilder.AddColumn<bool>(
            name: "CurrencyDisplayOptions_IncludeVatInSalesPrice",
            table: "Stores",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "CategoryPricingOptions",
            columns: table => new
            {
                PricingOptionsStoreId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CategoryId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProfitMarginRate = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CategoryPricingOptions", x => new { x.PricingOptionsStoreId, x.Id });
                table.ForeignKey(
                    name: "FK_CategoryPricingOptions_Stores_PricingOptionsStoreId",
                    column: x => x.PricingOptionsStoreId,
                    principalTable: "Stores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "VatRates",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Factor = table.Column<double>(type: "float", nullable: false),
                Factor2 = table.Column<double>(type: "float", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VatRates", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CategoryPricingOptions");

        migrationBuilder.DropTable(
            name: "VatRates");

        migrationBuilder.DropColumn(
            name: "CurrencyDisplayOptions_IncludeVatInSalesPrice",
            table: "Stores");

        migrationBuilder.AddColumn<decimal>(
            name: "Vat",
            table: "Products",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);
    }
}