using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations;

/// <inheritdoc />
public partial class AddProperties : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "BrandId",
            table: "Products",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "GTIN",
            table: "Products",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Headline",
            table: "Products",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "StoreId",
            table: "ProductCategories",
            type: "nvarchar(450)",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Products_BrandId",
            table: "Products",
            column: "BrandId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductCategories_StoreId",
            table: "ProductCategories",
            column: "StoreId");

        migrationBuilder.AddForeignKey(
            name: "FK_ProductCategories_Stores_StoreId",
            table: "ProductCategories",
            column: "StoreId",
            principalTable: "Stores",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Products_Brands_BrandId",
            table: "Products",
            column: "BrandId",
            principalTable: "Brands",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ProductCategories_Stores_StoreId",
            table: "ProductCategories");

        migrationBuilder.DropForeignKey(
            name: "FK_Products_Brands_BrandId",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_Products_BrandId",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_ProductCategories_StoreId",
            table: "ProductCategories");

        migrationBuilder.DropColumn(
            name: "BrandId",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "GTIN",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "Headline",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "StoreId",
            table: "ProductCategories");
    }
}