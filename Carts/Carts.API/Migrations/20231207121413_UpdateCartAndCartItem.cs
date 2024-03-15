using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Carts.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartAndCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Carts",
                newName: "Tag");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "Tag",
                table: "Carts",
                newName: "Name");
        }
    }
}