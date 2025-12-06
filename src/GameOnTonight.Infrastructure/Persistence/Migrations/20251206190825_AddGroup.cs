using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GameOnTonight.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "GameSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupMemberId",
                table: "GameSessionPlayers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "BoardGames",
                type: "integer",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Profiles_UserId",
                table: "Profiles",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupInviteCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInviteCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupInviteCodes_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Profiles_UserId",
                        column: x => x.UserId,
                        principalTable: "Profiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_GroupId",
                table: "GameSessions",
                column: "GroupId",
                filter: "\"GroupId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessionPlayers_GroupMemberId",
                table: "GameSessionPlayers",
                column: "GroupMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGames_GroupId",
                table: "BoardGames",
                column: "GroupId",
                filter: "\"GroupId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInviteCodes_Code",
                table: "GroupInviteCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupInviteCodes_ExpiresAt",
                table: "GroupInviteCodes",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInviteCodes_GroupId",
                table: "GroupInviteCodes",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId_UserId",
                table: "GroupMembers",
                columns: new[] { "GroupId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_UserId",
                table: "GroupMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardGames_Groups_GroupId",
                table: "BoardGames",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessionPlayers_GroupMembers_GroupMemberId",
                table: "GameSessionPlayers",
                column: "GroupMemberId",
                principalTable: "GroupMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessions_Groups_GroupId",
                table: "GameSessions",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardGames_Groups_GroupId",
                table: "BoardGames");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSessionPlayers_GroupMembers_GroupMemberId",
                table: "GameSessionPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_Groups_GroupId",
                table: "GameSessions");

            migrationBuilder.DropTable(
                name: "GroupInviteCodes");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Profiles_UserId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_GameSessions_GroupId",
                table: "GameSessions");

            migrationBuilder.DropIndex(
                name: "IX_GameSessionPlayers_GroupMemberId",
                table: "GameSessionPlayers");

            migrationBuilder.DropIndex(
                name: "IX_BoardGames_GroupId",
                table: "BoardGames");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "GroupMemberId",
                table: "GameSessionPlayers");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "BoardGames");
        }
    }
}
