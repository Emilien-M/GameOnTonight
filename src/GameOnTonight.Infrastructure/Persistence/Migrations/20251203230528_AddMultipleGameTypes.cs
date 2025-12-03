using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GameOnTonight.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleGameTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BoardGames_GameType",
                table: "BoardGames");

            migrationBuilder.DropColumn(
                name: "GameType",
                table: "BoardGames");

            migrationBuilder.CreateTable(
                name: "GameTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTypes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardGameGameTypes",
                columns: table => new
                {
                    BoardGameId = table.Column<int>(type: "integer", nullable: false),
                    GameTypeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardGameGameTypes", x => new { x.BoardGameId, x.GameTypeId });
                    table.ForeignKey(
                        name: "FK_BoardGameGameTypes_BoardGames_BoardGameId",
                        column: x => x.BoardGameId,
                        principalTable: "BoardGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardGameGameTypes_GameTypes_GameTypeId",
                        column: x => x.GameTypeId,
                        principalTable: "GameTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardGameGameTypes_GameTypeId",
                table: "BoardGameGameTypes",
                column: "GameTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTypes_UserId",
                table: "GameTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTypes_UserId_Name",
                table: "GameTypes",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardGameGameTypes");

            migrationBuilder.DropTable(
                name: "GameTypes");

            migrationBuilder.AddColumn<string>(
                name: "GameType",
                table: "BoardGames",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BoardGames_GameType",
                table: "BoardGames",
                column: "GameType");
        }
    }
}
