using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Carts.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "Carts",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Updated",
                table: "Carts",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Updated",
                table: "CartItems",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "CartItems");
        }
    }
}
