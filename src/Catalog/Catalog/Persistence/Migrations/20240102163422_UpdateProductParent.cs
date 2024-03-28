using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductParent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Products_ParentProductId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ParentProductId",
                table: "Products",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ParentProductId",
                table: "Products",
                newName: "IX_Products_ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Products_ParentId",
                table: "Products",
                column: "ParentId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Products_ParentId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Products",
                newName: "ParentProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ParentId",
                table: "Products",
                newName: "IX_Products_ParentProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Products_ParentProductId",
                table: "Products",
                column: "ParentProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
