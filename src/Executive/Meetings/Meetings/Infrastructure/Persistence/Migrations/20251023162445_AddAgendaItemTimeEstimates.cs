using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourBrand.Meetings.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAgendaItemTimeEstimates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendeeRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CanVote = table.Column<bool>(type: "bit", nullable: false),
                    CanSpeak = table.Column<bool>(type: "bit", nullable: false),
                    CanPropose = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendeeRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessageConsumers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Consumer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessageConsumers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AgendaItemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    HandledByRoleId = table.Column<int>(type: "int", nullable: true),
                    RequiresDiscussion = table.Column<bool>(type: "bit", nullable: false),
                    RequiresVoting = table.Column<bool>(type: "bit", nullable: false),
                    CanBePostponed = table.Column<bool>(type: "bit", nullable: false),
                    CanBeSkipped = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendaItemTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgendaItemTypes_AttendeeRoles_HandledByRoleId",
                        column: x => x.HandledByRoleId,
                        principalTable: "AttendeeRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MeetingGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quorum_RequiredNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingGroups", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_MeetingGroups_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MeetingGroups_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    AdjournedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AdjournmentMessage = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CurrentAgendaItemIndex = table.Column<int>(type: "int", nullable: true),
                    CurrentAgendaSubItemIndex = table.Column<int>(type: "int", nullable: true),
                    Quorum_RequiredNumber = table.Column<int>(type: "int", nullable: false),
                    SpeakingRightsAccessLevel = table.Column<int>(type: "int", nullable: false),
                    VotingRightsAccessLevel = table.Column<int>(type: "int", nullable: false),
                    CanAnyoneJoin = table.Column<bool>(type: "bit", nullable: false),
                    JoinAsId = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CanceledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ShowAgendaTimeEstimates = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Meetings_AttendeeRoles_JoinAsId",
                        column: x => x.JoinAsId,
                        principalTable: "AttendeeRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Meetings_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Meetings_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Motion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motion", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Motion_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Motion_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Organizations_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MeetingGroupMembers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeetingGroupId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    HasSpeakingRights = table.Column<bool>(type: "bit", nullable: true),
                    HasVotingRights = table.Column<bool>(type: "bit", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingGroupMembers", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_MeetingGroupMembers_AttendeeRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AttendeeRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingGroupMembers_MeetingGroups_OrganizationId_MeetingGroupId",
                        columns: x => new { x.OrganizationId, x.MeetingGroupId },
                        principalTable: "MeetingGroups",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingGroupMembers_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MeetingGroupMembers_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MeetingAttendees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeetingId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    MeetingGroupId = table.Column<int>(type: "int", nullable: true),
                    MeetingGroupMemberId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RemovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    InvitedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    InviteAcceptedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    JoinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsPresent = table.Column<bool>(type: "bit", nullable: false),
                    HasSpeakingRights = table.Column<bool>(type: "bit", nullable: true),
                    HasVotingRights = table.Column<bool>(type: "bit", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingAttendees", x => new { x.OrganizationId, x.MeetingId, x.Id });
                    table.ForeignKey(
                        name: "FK_MeetingAttendees_AttendeeRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AttendeeRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MeetingAttendees_Meetings_OrganizationId_MeetingId",
                        columns: x => new { x.OrganizationId, x.MeetingId },
                        principalTable: "Meetings",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingAttendees_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MeetingAttendees_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Minutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeetingId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Started = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Canceled = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Ended = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Minutes", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Minutes_Meetings_OrganizationId_MeetingId",
                        columns: x => new { x.OrganizationId, x.MeetingId },
                        principalTable: "Meetings",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Minutes_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Minutes_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MotionOperativeClauses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MotionId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotionOperativeClauses", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_MotionOperativeClauses_Motion_OrganizationId_MotionId",
                        columns: x => new { x.OrganizationId, x.MotionId },
                        principalTable: "Motion",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MotionOperativeClauses_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MotionOperativeClauses_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrganizationUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeetingId = table.Column<int>(type: "int", nullable: false),
                    MinutesId = table.Column<int>(type: "int", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RejectedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendas", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Agendas_Meetings_OrganizationId_MeetingId",
                        columns: x => new { x.OrganizationId, x.MeetingId },
                        principalTable: "Meetings",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendas_Minutes_OrganizationId_MinutesId",
                        columns: x => new { x.OrganizationId, x.MinutesId },
                        principalTable: "Minutes",
                        principalColumns: new[] { "OrganizationId", "Id" });
                    table.ForeignKey(
                        name: "FK_Agendas_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Agendas_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MinutesAttendees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MinutesId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    MeetingGroupId = table.Column<int>(type: "int", nullable: true),
                    MeetingGroupMemberId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPresent = table.Column<bool>(type: "bit", nullable: false),
                    HasSpeakingRights = table.Column<bool>(type: "bit", nullable: true),
                    HasVotingRights = table.Column<bool>(type: "bit", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinutesAttendees", x => new { x.OrganizationId, x.MinutesId, x.Id });
                    table.ForeignKey(
                        name: "FK_MinutesAttendees_AttendeeRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AttendeeRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MinutesAttendees_Minutes_OrganizationId_MinutesId",
                        columns: x => new { x.OrganizationId, x.MinutesId },
                        principalTable: "Minutes",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinutesAttendees_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MinutesAttendees_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MinutesItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MinutesId = table.Column<int>(type: "int", nullable: false),
                    AgendaId = table.Column<int>(type: "int", nullable: true),
                    AgendaItemId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    Heading = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    MotionId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinutesItems", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_MinutesItems_AgendaItemTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AgendaItemTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MinutesItems_Minutes_OrganizationId_MinutesId",
                        columns: x => new { x.OrganizationId, x.MinutesId },
                        principalTable: "Minutes",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinutesItems_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MinutesItems_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AgendaItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AgendaId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    DiscussionActions = table.Column<int>(type: "int", nullable: false),
                    VoteActions = table.Column<int>(type: "int", nullable: false),
                    EstimatedStartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EstimatedEndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EstimatedDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsDiscussionCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsVoteCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DiscussionStartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DiscussionEndedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    VotingStartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    VotingEndedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MotionId = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendaItems", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_AgendaItems_AgendaItemTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AgendaItemTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgendaItems_AgendaItems_OrganizationId_ParentId",
                        columns: x => new { x.OrganizationId, x.ParentId },
                        principalTable: "AgendaItems",
                        principalColumns: new[] { "OrganizationId", "Id" });
                    table.ForeignKey(
                        name: "FK_AgendaItems_Agendas_OrganizationId_AgendaId",
                        columns: x => new { x.OrganizationId, x.AgendaId },
                        principalTable: "Agendas",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgendaItems_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgendaItems_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Voting",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AgendaItemId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MotionOperativeClauseId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    HasPassed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voting", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Voting_AgendaItems_OrganizationId_AgendaItemId",
                        columns: x => new { x.OrganizationId, x.AgendaItemId },
                        principalTable: "AgendaItems",
                        principalColumns: new[] { "OrganizationId", "Id" });
                    table.ForeignKey(
                        name: "FK_Voting_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Voting_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VotingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VoterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeCast = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Option = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Votes_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Votes_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Votes_Voting_OrganizationId_VotingId",
                        columns: x => new { x.OrganizationId, x.VotingId },
                        principalTable: "Voting",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ballots",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ElectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VoterId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeCast = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SelectedCandidateId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ballots", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Ballots_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ballots_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Discussions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AgendaItemId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    SpeakingTimeLimit = table.Column<TimeSpan>(type: "time", nullable: true),
                    CurrentSpeakerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CurrentSpeakerClockStartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discussions", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Discussions_AgendaItems_OrganizationId_AgendaItemId",
                        columns: x => new { x.OrganizationId, x.AgendaItemId },
                        principalTable: "AgendaItems",
                        principalColumns: new[] { "OrganizationId", "Id" });
                    table.ForeignKey(
                        name: "FK_Discussions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Discussions_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SpeakerRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpeakerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttendeeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActualSpeakingTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    AllocatedSpeakingTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    HasExtendedSpeakingTime = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeakerRequests", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_SpeakerRequests_Discussions_OrganizationId_SpeakerId",
                        columns: x => new { x.OrganizationId, x.SpeakerId },
                        principalTable: "Discussions",
                        principalColumns: new[] { "OrganizationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpeakerRequests_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpeakerRequests_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ElectionCandidates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ElectionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupMemberId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttendeeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NominatedBy_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NominatedBy_UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NominatedBy_GroupMemberId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NominatedBy_AttendeeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NominatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Statement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WithdrawnAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsPreMeetingNomination = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionCandidates", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_ElectionCandidates_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ElectionCandidates_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Elections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeetingId = table.Column<int>(type: "int", nullable: true),
                    AgendaId = table.Column<int>(type: "int", nullable: true),
                    AgendaItemId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MotionOperativeClauseId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    PositionId = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    MinimumVotesToWin = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    ElectedCandidateId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elections", x => new { x.OrganizationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Elections_AgendaItems_OrganizationId_AgendaItemId",
                        columns: x => new { x.OrganizationId, x.AgendaItemId },
                        principalTable: "AgendaItems",
                        principalColumns: new[] { "OrganizationId", "Id" });
                    table.ForeignKey(
                        name: "FK_Elections_ElectionCandidates_OrganizationId_ElectedCandidateId",
                        columns: x => new { x.OrganizationId, x.ElectedCandidateId },
                        principalTable: "ElectionCandidates",
                        principalColumns: new[] { "OrganizationId", "Id" });
                    table.ForeignKey(
                        name: "FK_Elections_MemberRoles_PositionId",
                        column: x => x.PositionId,
                        principalTable: "MemberRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Elections_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Elections_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_CreatedById",
                table: "AgendaItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_LastModifiedById",
                table: "AgendaItems",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_OrganizationId",
                table: "AgendaItems",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_OrganizationId_AgendaId",
                table: "AgendaItems",
                columns: new[] { "OrganizationId", "AgendaId" });

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_OrganizationId_ParentId",
                table: "AgendaItems",
                columns: new[] { "OrganizationId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_TenantId",
                table: "AgendaItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItems_TypeId",
                table: "AgendaItems",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaItemTypes_HandledByRoleId",
                table: "AgendaItemTypes",
                column: "HandledByRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_CreatedById",
                table: "Agendas",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_LastModifiedById",
                table: "Agendas",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_OrganizationId",
                table: "Agendas",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_OrganizationId_MeetingId",
                table: "Agendas",
                columns: new[] { "OrganizationId", "MeetingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_OrganizationId_MinutesId",
                table: "Agendas",
                columns: new[] { "OrganizationId", "MinutesId" },
                unique: true,
                filter: "[MinutesId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_TenantId",
                table: "Agendas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Ballots_CreatedById",
                table: "Ballots",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Ballots_LastModifiedById",
                table: "Ballots",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Ballots_OrganizationId",
                table: "Ballots",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Ballots_OrganizationId_ElectionId",
                table: "Ballots",
                columns: new[] { "OrganizationId", "ElectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ballots_OrganizationId_SelectedCandidateId",
                table: "Ballots",
                columns: new[] { "OrganizationId", "SelectedCandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ballots_TenantId",
                table: "Ballots",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_CreatedById",
                table: "Discussions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_LastModifiedById",
                table: "Discussions",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_OrganizationId",
                table: "Discussions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_OrganizationId_AgendaItemId",
                table: "Discussions",
                columns: new[] { "OrganizationId", "AgendaItemId" },
                unique: true,
                filter: "[AgendaItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_OrganizationId_CurrentSpeakerId",
                table: "Discussions",
                columns: new[] { "OrganizationId", "CurrentSpeakerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_TenantId",
                table: "Discussions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidates_CreatedById",
                table: "ElectionCandidates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidates_LastModifiedById",
                table: "ElectionCandidates",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidates_OrganizationId",
                table: "ElectionCandidates",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidates_OrganizationId_ElectionId",
                table: "ElectionCandidates",
                columns: new[] { "OrganizationId", "ElectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidates_TenantId",
                table: "ElectionCandidates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Elections_CreatedById",
                table: "Elections",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Elections_LastModifiedById",
                table: "Elections",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Elections_OrganizationId",
                table: "Elections",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Elections_OrganizationId_AgendaItemId",
                table: "Elections",
                columns: new[] { "OrganizationId", "AgendaItemId" },
                unique: true,
                filter: "[AgendaItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Elections_OrganizationId_ElectedCandidateId",
                table: "Elections",
                columns: new[] { "OrganizationId", "ElectedCandidateId" });

            migrationBuilder.CreateIndex(
                name: "IX_Elections_PositionId",
                table: "Elections",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Elections_TenantId",
                table: "Elections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendees_CreatedById",
                table: "MeetingAttendees",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendees_LastModifiedById",
                table: "MeetingAttendees",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendees_OrganizationId",
                table: "MeetingAttendees",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendees_RoleId",
                table: "MeetingAttendees",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingAttendees_TenantId",
                table: "MeetingAttendees",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroupMembers_CreatedById",
                table: "MeetingGroupMembers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroupMembers_LastModifiedById",
                table: "MeetingGroupMembers",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroupMembers_OrganizationId",
                table: "MeetingGroupMembers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroupMembers_OrganizationId_MeetingGroupId",
                table: "MeetingGroupMembers",
                columns: new[] { "OrganizationId", "MeetingGroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroupMembers_RoleId",
                table: "MeetingGroupMembers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroupMembers_TenantId",
                table: "MeetingGroupMembers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroups_CreatedById",
                table: "MeetingGroups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroups_LastModifiedById",
                table: "MeetingGroups",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroups_OrganizationId",
                table: "MeetingGroups",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingGroups_TenantId",
                table: "MeetingGroups",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_CreatedById",
                table: "Meetings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_JoinAsId",
                table: "Meetings",
                column: "JoinAsId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_LastModifiedById",
                table: "Meetings",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_OrganizationId",
                table: "Meetings",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_TenantId",
                table: "Meetings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Minutes_CreatedById",
                table: "Minutes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Minutes_LastModifiedById",
                table: "Minutes",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Minutes_OrganizationId",
                table: "Minutes",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Minutes_OrganizationId_MeetingId",
                table: "Minutes",
                columns: new[] { "OrganizationId", "MeetingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Minutes_TenantId",
                table: "Minutes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesAttendees_CreatedById",
                table: "MinutesAttendees",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesAttendees_LastModifiedById",
                table: "MinutesAttendees",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesAttendees_OrganizationId",
                table: "MinutesAttendees",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesAttendees_RoleId",
                table: "MinutesAttendees",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesAttendees_TenantId",
                table: "MinutesAttendees",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesItems_CreatedById",
                table: "MinutesItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesItems_LastModifiedById",
                table: "MinutesItems",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesItems_OrganizationId",
                table: "MinutesItems",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesItems_OrganizationId_MinutesId",
                table: "MinutesItems",
                columns: new[] { "OrganizationId", "MinutesId" });

            migrationBuilder.CreateIndex(
                name: "IX_MinutesItems_TenantId",
                table: "MinutesItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MinutesItems_TypeId",
                table: "MinutesItems",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Motion_CreatedById",
                table: "Motion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Motion_LastModifiedById",
                table: "Motion",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Motion_OrganizationId",
                table: "Motion",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Motion_TenantId",
                table: "Motion",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionOperativeClauses_CreatedById",
                table: "MotionOperativeClauses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MotionOperativeClauses_LastModifiedById",
                table: "MotionOperativeClauses",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MotionOperativeClauses_OrganizationId",
                table: "MotionOperativeClauses",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_MotionOperativeClauses_OrganizationId_MotionId",
                table: "MotionOperativeClauses",
                columns: new[] { "OrganizationId", "MotionId" });

            migrationBuilder.CreateIndex(
                name: "IX_MotionOperativeClauses_TenantId",
                table: "MotionOperativeClauses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CreatedById",
                table: "Organizations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_LastModifiedById",
                table: "Organizations",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_TenantId",
                table: "Organizations",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_CreatedById",
                table: "OrganizationUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_LastModifiedById",
                table: "OrganizationUsers",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_OrganizationId",
                table: "OrganizationUsers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_TenantId",
                table: "OrganizationUsers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_UserId",
                table: "OrganizationUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeakerRequests_CreatedById",
                table: "SpeakerRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SpeakerRequests_LastModifiedById",
                table: "SpeakerRequests",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SpeakerRequests_OrganizationId",
                table: "SpeakerRequests",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeakerRequests_OrganizationId_SpeakerId",
                table: "SpeakerRequests",
                columns: new[] { "OrganizationId", "SpeakerId" });

            migrationBuilder.CreateIndex(
                name: "IX_SpeakerRequests_TenantId",
                table: "SpeakerRequests",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedById",
                table: "Users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastModifiedById",
                table: "Users",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_CreatedById",
                table: "Votes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_LastModifiedById",
                table: "Votes",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_OrganizationId",
                table: "Votes",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_OrganizationId_VotingId",
                table: "Votes",
                columns: new[] { "OrganizationId", "VotingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_TenantId",
                table: "Votes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Voting_CreatedById",
                table: "Voting",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Voting_LastModifiedById",
                table: "Voting",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Voting_OrganizationId",
                table: "Voting",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Voting_OrganizationId_AgendaItemId",
                table: "Voting",
                columns: new[] { "OrganizationId", "AgendaItemId" },
                unique: true,
                filter: "[AgendaItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Voting_TenantId",
                table: "Voting",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ballots_ElectionCandidates_OrganizationId_SelectedCandidateId",
                table: "Ballots",
                columns: new[] { "OrganizationId", "SelectedCandidateId" },
                principalTable: "ElectionCandidates",
                principalColumns: new[] { "OrganizationId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Ballots_Elections_OrganizationId_ElectionId",
                table: "Ballots",
                columns: new[] { "OrganizationId", "ElectionId" },
                principalTable: "Elections",
                principalColumns: new[] { "OrganizationId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Discussions_SpeakerRequests_OrganizationId_CurrentSpeakerId",
                table: "Discussions",
                columns: new[] { "OrganizationId", "CurrentSpeakerId" },
                principalTable: "SpeakerRequests",
                principalColumns: new[] { "OrganizationId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_ElectionCandidates_Elections_OrganizationId_ElectionId",
                table: "ElectionCandidates",
                columns: new[] { "OrganizationId", "ElectionId" },
                principalTable: "Elections",
                principalColumns: new[] { "OrganizationId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgendaItems_AgendaItemTypes_TypeId",
                table: "AgendaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AgendaItems_Agendas_OrganizationId_AgendaId",
                table: "AgendaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AgendaItems_Users_CreatedById",
                table: "AgendaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AgendaItems_Users_LastModifiedById",
                table: "AgendaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Discussions_Users_CreatedById",
                table: "Discussions");

            migrationBuilder.DropForeignKey(
                name: "FK_Discussions_Users_LastModifiedById",
                table: "Discussions");

            migrationBuilder.DropForeignKey(
                name: "FK_ElectionCandidates_Users_CreatedById",
                table: "ElectionCandidates");

            migrationBuilder.DropForeignKey(
                name: "FK_ElectionCandidates_Users_LastModifiedById",
                table: "ElectionCandidates");

            migrationBuilder.DropForeignKey(
                name: "FK_Elections_Users_CreatedById",
                table: "Elections");

            migrationBuilder.DropForeignKey(
                name: "FK_Elections_Users_LastModifiedById",
                table: "Elections");

            migrationBuilder.DropForeignKey(
                name: "FK_SpeakerRequests_Users_CreatedById",
                table: "SpeakerRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SpeakerRequests_Users_LastModifiedById",
                table: "SpeakerRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Elections_ElectionCandidates_OrganizationId_ElectedCandidateId",
                table: "Elections");

            migrationBuilder.DropForeignKey(
                name: "FK_Discussions_AgendaItems_OrganizationId_AgendaItemId",
                table: "Discussions");

            migrationBuilder.DropForeignKey(
                name: "FK_Discussions_SpeakerRequests_OrganizationId_CurrentSpeakerId",
                table: "Discussions");

            migrationBuilder.DropTable(
                name: "Ballots");

            migrationBuilder.DropTable(
                name: "MeetingAttendees");

            migrationBuilder.DropTable(
                name: "MeetingGroupMembers");

            migrationBuilder.DropTable(
                name: "MinutesAttendees");

            migrationBuilder.DropTable(
                name: "MinutesItems");

            migrationBuilder.DropTable(
                name: "MotionOperativeClauses");

            migrationBuilder.DropTable(
                name: "OrganizationUsers");

            migrationBuilder.DropTable(
                name: "OutboxMessageConsumers");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "MeetingGroups");

            migrationBuilder.DropTable(
                name: "Motion");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Voting");

            migrationBuilder.DropTable(
                name: "AgendaItemTypes");

            migrationBuilder.DropTable(
                name: "Agendas");

            migrationBuilder.DropTable(
                name: "Minutes");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropTable(
                name: "AttendeeRoles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ElectionCandidates");

            migrationBuilder.DropTable(
                name: "Elections");

            migrationBuilder.DropTable(
                name: "MemberRoles");

            migrationBuilder.DropTable(
                name: "AgendaItems");

            migrationBuilder.DropTable(
                name: "SpeakerRequests");

            migrationBuilder.DropTable(
                name: "Discussions");
        }
    }
}
