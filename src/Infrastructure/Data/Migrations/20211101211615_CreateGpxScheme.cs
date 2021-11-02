using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Heracles.Infrastructure.Data.Migrations
{
    public partial class CreateGpxScheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    ActivityType = table.Column<int>(type: "int", nullable: false),
                    Elevation = table.Column<double>(type: "float", nullable: true),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Pace = table.Column<TimeSpan>(type: "time", nullable: false),
                    Speed = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackSegments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExerciseSessionId = table.Column<int>(type: "int", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    Elevation = table.Column<double>(type: "float", nullable: true),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    GpxTrackId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackSegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackSegments_Tracks_GpxTrackId",
                        column: x => x.GpxTrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Elevation = table.Column<double>(type: "float", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrackSegmentId = table.Column<int>(type: "int", nullable: false),
                    GpxTrackSegmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackPoints_TrackSegments_GpxTrackSegmentId",
                        column: x => x.GpxTrackSegmentId,
                        principalTable: "TrackSegments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackPoints_GpxTrackSegmentId",
                table: "TrackPoints",
                column: "GpxTrackSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackSegments_GpxTrackId",
                table: "TrackSegments",
                column: "GpxTrackId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackPoints");

            migrationBuilder.DropTable(
                name: "TrackSegments");

            migrationBuilder.DropTable(
                name: "Tracks");
        }
    }
}
