using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.StoreFront.API.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "StoreFront",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StoreFront", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CartItems",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ProductId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                RegularPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                Quantity = table.Column<double>(type: "float", nullable: false),
                Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CartId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CartItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_CartItems_StoreFront_CartId",
                    column: x => x.CartId,
                    principalTable: "StoreFront",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_CartItems_CartId",
            table: "CartItems",
            column: "CartId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CartItems");

        migrationBuilder.DropTable(
            name: "StoreFront");
    }
}