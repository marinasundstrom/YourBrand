using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Carts.API.Migrations;

/// <inheritdoc />
public partial class ChangeProductIdToLong : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<long>(
            name: "ProductId",
            table: "CartItems",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "ProductId",
            table: "CartItems",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);
    }
}