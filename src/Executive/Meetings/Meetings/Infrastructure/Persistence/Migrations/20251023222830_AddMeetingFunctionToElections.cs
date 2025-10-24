using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Meetings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingFunctionToElections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgendaItemTypes_AttendeeRoles_HandledByRoleId",
                table: "AgendaItemTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Elections_MemberRoles_PositionId",
                table: "Elections");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AttendeeRoles_JoinAsId",
                table: "Meetings");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "Elections",
                newName: "MeetingFunctionId");

            migrationBuilder.RenameIndex(
                name: "IX_Elections_PositionId",
                table: "Elections",
                newName: "IX_Elections_MeetingFunctionId");

            migrationBuilder.RenameColumn(
                name: "HandledByRoleId",
                table: "AgendaItemTypes",
                newName: "HandledByFunctionId");

            migrationBuilder.RenameIndex(
                name: "IX_AgendaItemTypes_HandledByRoleId",
                table: "AgendaItemTypes",
                newName: "IX_AgendaItemTypes_HandledByFunctionId");

            migrationBuilder.CreateTable(
                name: "MeetingFunctions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingFunctions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeetingAttendeeFunctions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeetingId = table.Column<int>(type: "int", nullable: false),
                    MeetingAttendeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeetingFunctionId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingAttendeeFunctions", x => new { x.OrganizationId, x.MeetingId, x.MeetingAttendeeId, x.Id });
                    table.ForeignKey(
                        name: "FK_MeetingAttendeeFunctions_MeetingAttendees_OrganizationId_MeetingId_MeetingAttendeeId",
                        columns: x => new { x.OrganizationId, x.MeetingId, x.MeetingAttendeeId },
                        principalTable: "MeetingAttendees",
                        principalColumns: new[] { "OrganizationId", "MeetingId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingAttendeeFunctions_MeetingFunctions_MeetingFunctionId",
                        column: x => x.MeetingFunctionId,
                        principalTable: "MeetingFunctions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MeetingAttendeeFunctions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MeetingAttendeeFunctions_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendeeFunctions_CreatedById",
                table: "MeetingAttendeeFunctions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendeeFunctions_LastModifiedById",
                table: "MeetingAttendeeFunctions",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendeeFunctions_MeetingFunctionId",
                table: "MeetingAttendeeFunctions",
                column: "MeetingFunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendeeFunctions_OrganizationId",
                table: "MeetingAttendeeFunctions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendeeFunctions_TenantId",
                table: "MeetingAttendeeFunctions",
                column: "TenantId");

            migrationBuilder.CreateTable(
                name: "MinutesTasks",
                columns: table => new
                {
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MinutesId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedToId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AssignedToName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AssignedToEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DueAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CompletedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinutesTasks", x => new { x.OrganizationId, x.MinutesId, x.Id });
                    table.ForeignKey(
                        name: "FK_MinutesTasks_Minutes_OrganizationId_MinutesId",
                        columns: x => new { x.OrganizationId, x.MinutesId },
                        principalTable: "Minutes",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinutesTasks_Users_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MinutesTasks_Users_CompletedById",
                        column: x => x.CompletedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MinutesTasks_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MinutesTasks_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MinutesTasks_AssignedToId",
                table: "MinutesTasks",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesTasks_CompletedById",
                table: "MinutesTasks",
                column: "CompletedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesTasks_CreatedById",
                table: "MinutesTasks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesTasks_LastModifiedById",
                table: "MinutesTasks",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesTasks_TenantId",
                table: "MinutesTasks",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaItemTypes_MeetingFunctions_HandledByFunctionId",
                table: "AgendaItemTypes",
                column: "HandledByFunctionId",
                principalTable: "MeetingFunctions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Elections_MeetingFunctions_MeetingFunctionId",
                table: "Elections",
                column: "MeetingFunctionId",
                principalTable: "MeetingFunctions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AttendeeRoles_JoinAsId",
                table: "Meetings",
                column: "JoinAsId",
                principalTable: "AttendeeRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgendaItemTypes_MeetingFunctions_HandledByFunctionId",
                table: "AgendaItemTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Elections_MeetingFunctions_MeetingFunctionId",
                table: "Elections");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AttendeeRoles_JoinAsId",
                table: "Meetings");

            migrationBuilder.DropTable(
                name: "MinutesTasks");

            migrationBuilder.DropTable(
                name: "MeetingAttendeeFunctions");

            migrationBuilder.DropTable(
                name: "MeetingFunctions");

            migrationBuilder.RenameColumn(
                name: "MeetingFunctionId",
                table: "Elections",
                newName: "PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_Elections_MeetingFunctionId",
                table: "Elections",
                newName: "IX_Elections_PositionId");

            migrationBuilder.RenameColumn(
                name: "HandledByFunctionId",
                table: "AgendaItemTypes",
                newName: "HandledByRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AgendaItemTypes_HandledByFunctionId",
                table: "AgendaItemTypes",
                newName: "IX_AgendaItemTypes_HandledByRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgendaItemTypes_AttendeeRoles_HandledByRoleId",
                table: "AgendaItemTypes",
                column: "HandledByRoleId",
                principalTable: "AttendeeRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Elections_MemberRoles_PositionId",
                table: "Elections",
                column: "PositionId",
                principalTable: "MemberRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AttendeeRoles_JoinAsId",
                table: "Meetings",
                column: "JoinAsId",
                principalTable: "AttendeeRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
