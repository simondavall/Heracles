using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Heracles.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackPointData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackPointData",
                columns: table => new
                {
                    TrackId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Seq = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackPointId = table.Column<int>(type: "INTEGER", nullable: false),
                    CumulativeDistance = table.Column<double>(type: "REAL", nullable: false),
                    CumulativeTime = table.Column<int>(type: "INTEGER", nullable: false),
                    Elevation = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackPointData", x => new { x.TrackId, x.Seq });
                    table.ForeignKey(
                        name: "FK_TrackPointData_TrackPoints_TrackPointId",
                        column: x => x.TrackPointId,
                        principalTable: "TrackPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackPointData_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackPointData_TrackPointId",
                table: "TrackPointData",
                column: "TrackPointId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackPointData");
        }
    }
}
