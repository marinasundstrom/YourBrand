using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations;

/// <inheritdoc />
public partial class UpdateProductImage2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ProductImages_Products_ProductId",
            table: "ProductImages");

        migrationBuilder.AlterColumn<long>(
            name: "ProductId",
            table: "ProductImages",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.AddForeignKey(
            name: "FK_ProductImages_Products_ProductId",
            table: "ProductImages",
            column: "ProductId",
            principalTable: "Products",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ProductImages_Products_ProductId",
            table: "ProductImages");

        migrationBuilder.AlterColumn<long>(
            name: "ProductId",
            table: "ProductImages",
            type: "bigint",
            nullable: false,
            defaultValue: 0L,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_ProductImages_Products_ProductId",
            table: "ProductImages",
            column: "ProductId",
            principalTable: "Products",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}