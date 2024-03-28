using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Catalog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntityConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SelectableOption_Price",
                table: "Options",
                newName: "NumericalValueOption_Price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumericalValueOption_Price",
                table: "Options",
                newName: "SelectableOption_Price");
        }
    }
}
