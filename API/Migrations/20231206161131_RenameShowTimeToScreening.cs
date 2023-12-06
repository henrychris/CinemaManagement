using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RenameShowTimeToScreening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_ShowTimes_ShowtimeId",
                schema: "CinemaDb",
                table: "Seats");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Theaters_TheaterId",
                schema: "CinemaDb",
                table: "Seats");

            migrationBuilder.DropTable(
                name: "ShowTimes",
                schema: "CinemaDb");

            migrationBuilder.DropIndex(
                name: "IX_Seats_TheaterId",
                schema: "CinemaDb",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "TheaterId",
                schema: "CinemaDb",
                table: "Seats");

            migrationBuilder.RenameColumn(
                name: "ShowtimeId",
                schema: "CinemaDb",
                table: "Seats",
                newName: "ScreeningId");

            migrationBuilder.RenameIndex(
                name: "IX_Seats_ShowtimeId",
                schema: "CinemaDb",
                table: "Seats",
                newName: "IX_Seats_ScreeningId");

            migrationBuilder.AlterColumn<string>(
                name: "ScreenType",
                schema: "CinemaDb",
                table: "Theaters",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "CinemaDb",
                table: "Theaters",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Screenings",
                schema: "CinemaDb",
                columns: table => new
                {
                    ShowtimeId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    ScreeningDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MovieId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TheatreId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TheaterId = table.Column<string>(type: "character varying(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Screenings", x => x.ShowtimeId);
                    table.ForeignKey(
                        name: "FK_Screenings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalSchema: "CinemaDb",
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Screenings_Theaters_TheaterId",
                        column: x => x.TheaterId,
                        principalSchema: "CinemaDb",
                        principalTable: "Theaters",
                        principalColumn: "TheaterId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Screenings_MovieId",
                schema: "CinemaDb",
                table: "Screenings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Screenings_TheaterId",
                schema: "CinemaDb",
                table: "Screenings",
                column: "TheaterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Screenings_ScreeningId",
                schema: "CinemaDb",
                table: "Seats",
                column: "ScreeningId",
                principalSchema: "CinemaDb",
                principalTable: "Screenings",
                principalColumn: "ShowtimeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Screenings_ScreeningId",
                schema: "CinemaDb",
                table: "Seats");

            migrationBuilder.DropTable(
                name: "Screenings",
                schema: "CinemaDb");

            migrationBuilder.RenameColumn(
                name: "ScreeningId",
                schema: "CinemaDb",
                table: "Seats",
                newName: "ShowtimeId");

            migrationBuilder.RenameIndex(
                name: "IX_Seats_ScreeningId",
                schema: "CinemaDb",
                table: "Seats",
                newName: "IX_Seats_ShowtimeId");

            migrationBuilder.AlterColumn<string>(
                name: "ScreenType",
                schema: "CinemaDb",
                table: "Theaters",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "CinemaDb",
                table: "Theaters",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "TheaterId",
                schema: "CinemaDb",
                table: "Seats",
                type: "character varying(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShowTimes",
                schema: "CinemaDb",
                columns: table => new
                {
                    ShowtimeId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    MovieId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TheaterId = table.Column<string>(type: "character varying(450)", nullable: true),
                    ScreeningDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TheatreId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowTimes", x => x.ShowtimeId);
                    table.ForeignKey(
                        name: "FK_ShowTimes_Movies_MovieId",
                        column: x => x.MovieId,
                        principalSchema: "CinemaDb",
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowTimes_Theaters_TheaterId",
                        column: x => x.TheaterId,
                        principalSchema: "CinemaDb",
                        principalTable: "Theaters",
                        principalColumn: "TheaterId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seats_TheaterId",
                schema: "CinemaDb",
                table: "Seats",
                column: "TheaterId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowTimes_MovieId",
                schema: "CinemaDb",
                table: "ShowTimes",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowTimes_TheaterId",
                schema: "CinemaDb",
                table: "ShowTimes",
                column: "TheaterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_ShowTimes_ShowtimeId",
                schema: "CinemaDb",
                table: "Seats",
                column: "ShowtimeId",
                principalSchema: "CinemaDb",
                principalTable: "ShowTimes",
                principalColumn: "ShowtimeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Theaters_TheaterId",
                schema: "CinemaDb",
                table: "Seats",
                column: "TheaterId",
                principalSchema: "CinemaDb",
                principalTable: "Theaters",
                principalColumn: "TheaterId");
        }
    }
}
