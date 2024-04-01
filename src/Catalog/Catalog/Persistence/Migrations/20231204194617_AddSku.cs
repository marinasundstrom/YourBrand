using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations;

/// <inheritdoc />
public partial class AddSku : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "SKU",
            table: "Products",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "SKU",
            table: "Products");
    }
}