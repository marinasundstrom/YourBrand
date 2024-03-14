using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoreModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricingOptions_ProfitMarginPercentage",
                table: "Stores",
                newName: "PricingOptions_ProfitMarginRate");

            migrationBuilder.AddColumn<int>(
                name: "PricingOptions_DefaultVatRateId",
                table: "Stores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_PricingOptions_DefaultVatRateId",
                table: "Stores",
                column: "PricingOptions_DefaultVatRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_VatRates_PricingOptions_DefaultVatRateId",
                table: "Stores",
                column: "PricingOptions_DefaultVatRateId",
                principalTable: "VatRates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_VatRates_PricingOptions_DefaultVatRateId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_PricingOptions_DefaultVatRateId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PricingOptions_DefaultVatRateId",
                table: "Stores");

            migrationBuilder.RenameColumn(
                name: "PricingOptions_ProfitMarginRate",
                table: "Stores",
                newName: "PricingOptions_ProfitMarginPercentage");
        }
    }
}
