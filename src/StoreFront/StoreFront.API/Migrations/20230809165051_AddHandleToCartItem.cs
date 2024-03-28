using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.StoreFront.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHandleToCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductHandle",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductHandle",
                table: "CartItems");
        }
    }
}