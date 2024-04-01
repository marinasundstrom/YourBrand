using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations;

/// <inheritdoc />
public partial class AddDefaultVatRateToCategoryPricingOptions : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<double>(
            name: "PricingOptions_ProfitMarginRate",
            table: "Stores",
            type: "float",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AddColumn<int>(
            name: "DefaultVatRateId",
            table: "CategoryPricingOptions",
            type: "int",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_CategoryPricingOptions_DefaultVatRateId",
            table: "CategoryPricingOptions",
            column: "DefaultVatRateId");

        migrationBuilder.AddForeignKey(
            name: "FK_CategoryPricingOptions_VatRates_DefaultVatRateId",
            table: "CategoryPricingOptions",
            column: "DefaultVatRateId",
            principalTable: "VatRates",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_CategoryPricingOptions_VatRates_DefaultVatRateId",
            table: "CategoryPricingOptions");

        migrationBuilder.DropIndex(
            name: "IX_CategoryPricingOptions_DefaultVatRateId",
            table: "CategoryPricingOptions");

        migrationBuilder.DropColumn(
            name: "DefaultVatRateId",
            table: "CategoryPricingOptions");

        migrationBuilder.AlterColumn<double>(
            name: "PricingOptions_ProfitMarginRate",
            table: "Stores",
            type: "float",
            nullable: false,
            defaultValue: 0.0,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);
    }
}