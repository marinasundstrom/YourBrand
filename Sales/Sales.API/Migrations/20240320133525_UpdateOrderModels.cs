using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Sales.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "OrderItems",
                newName: "UnitId");

            migrationBuilder.AlterColumn<double>(
                name: "VatRate",
                table: "Orders",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "Rounded",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Rounding",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rounded",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Rounding",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "OrderItems",
                newName: "ItemId");

            migrationBuilder.AlterColumn<double>(
                name: "VatRate",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
