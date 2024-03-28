using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Brands",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_ParentId",
                table: "Brands",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_Brands_ParentId",
                table: "Brands",
                column: "ParentId",
                principalTable: "Brands",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_Brands_ParentId",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Brands_ParentId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Brands");
        }
    }
}
