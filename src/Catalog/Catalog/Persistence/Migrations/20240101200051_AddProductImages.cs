using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations;

/// <inheritdoc />
public partial class AddProductImages : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProductImages",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                StoreId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                ProductId = table.Column<long>(type: "bigint", nullable: false),
                Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductImages", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductImages_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ProductImages_Stores_StoreId",
                    column: x => x.StoreId,
                    principalTable: "Stores",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_ProductImages_ProductId",
            table: "ProductImages",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductImages_StoreId",
            table: "ProductImages",
            column: "StoreId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProductImages");
    }
}