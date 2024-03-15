using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Carts.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVatRateAndDiscountRateToCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountRate",
                table: "CartItems",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "VatRate",
                table: "CartItems",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "CartItems");
        }
    }
}
