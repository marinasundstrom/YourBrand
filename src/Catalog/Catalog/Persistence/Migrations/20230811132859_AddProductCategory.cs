using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations;

/// <inheritdoc />
public partial class AddProductCategory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(
            name: "CategoryId",
            table: "Products",
            type: "bigint",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "ProductCategories",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ParentId = table.Column<long>(type: "bigint", nullable: true),
                CanAddProducts = table.Column<bool>(type: "bit", nullable: false),
                ProductsCount = table.Column<long>(type: "bigint", nullable: false),
                Handle = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Path = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductCategories", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductCategories_ProductCategories_ParentId",
                    column: x => x.ParentId,
                    principalTable: "ProductCategories",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Products_CategoryId",
            table: "Products",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductCategories_Handle",
            table: "ProductCategories",
            column: "Handle");

        migrationBuilder.CreateIndex(
            name: "IX_ProductCategories_ParentId",
            table: "ProductCategories",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_ProductCategories_Path",
            table: "ProductCategories",
            column: "Path");

        migrationBuilder.AddForeignKey(
            name: "FK_Products_ProductCategories_CategoryId",
            table: "Products",
            column: "CategoryId",
            principalTable: "ProductCategories",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Products_ProductCategories_CategoryId",
            table: "Products");

        migrationBuilder.DropTable(
            name: "ProductCategories");

        migrationBuilder.DropIndex(
            name: "IX_Products_CategoryId",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "CategoryId",
            table: "Products");
    }
}