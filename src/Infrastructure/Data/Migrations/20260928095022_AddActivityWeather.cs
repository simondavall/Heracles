using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heracles.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityWeather : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityWeather",
                columns: table => new
                {
                    TrackId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Temperature = table.Column<double>(type: "REAL", nullable: true),
                    FeelsLike = table.Column<double>(type: "REAL", nullable: true),
                    Conditions = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    WeatherCode = table.Column<int>(type: "INTEGER", nullable: true),
                    ObservationTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityWeather", x => x.TrackId);
                    table.ForeignKey(
                        name: "FK_ActivityWeather_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityWeather");
        }
    }
}
