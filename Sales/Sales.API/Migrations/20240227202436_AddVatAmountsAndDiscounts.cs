using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Sales.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVatAmountsAndDiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discounts",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VatAmounts",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discounts",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VatAmounts",
                table: "Orders");
        }
    }
}
