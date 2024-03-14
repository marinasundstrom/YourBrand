using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeForImageOnProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductImages_ImageId",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductImages_ImageId",
                table: "Products",
                column: "ImageId",
                principalTable: "ProductImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductImages_ImageId",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductImages_ImageId",
                table: "Products",
                column: "ImageId",
                principalTable: "ProductImages",
                principalColumn: "Id");
        }
    }
}
