using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations;

/// <inheritdoc />
public partial class UpdateProductImage : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Image",
            table: "Products");

        migrationBuilder.AddColumn<string>(
            name: "ImageId",
            table: "Products",
            type: "nvarchar(450)",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Products_ImageId",
            table: "Products",
            column: "ImageId");

        migrationBuilder.AddForeignKey(
            name: "FK_Products_ProductImages_ImageId",
            table: "Products",
            column: "ImageId",
            principalTable: "ProductImages",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Products_ProductImages_ImageId",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_Products_ImageId",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "ImageId",
            table: "Products");

        migrationBuilder.AddColumn<string>(
            name: "Image",
            table: "Products",
            type: "nvarchar(max)",
            nullable: true);
    }
}