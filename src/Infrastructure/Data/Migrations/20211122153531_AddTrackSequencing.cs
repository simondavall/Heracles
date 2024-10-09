using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Heracles.Infrastructure.Data.Migrations
{
    public partial class AddTrackSequencing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackPoints_TrackSegments_TrackSegmentId",
                table: "TrackPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackSegments_Tracks_TrackId",
                table: "TrackSegments");

            migrationBuilder.AlterColumn<Guid>(
                name: "TrackId",
                table: "TrackSegments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Seq",
                table: "TrackSegments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "TrackSegmentId",
                table: "TrackPoints",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Seq",
                table: "TrackPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackPoints_TrackSegments_TrackSegmentId",
                table: "TrackPoints",
                column: "TrackSegmentId",
                principalTable: "TrackSegments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackSegments_Tracks_TrackId",
                table: "TrackSegments",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackPoints_TrackSegments_TrackSegmentId",
                table: "TrackPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackSegments_Tracks_TrackId",
                table: "TrackSegments");

            migrationBuilder.DropColumn(
                name: "Seq",
                table: "TrackSegments");

            migrationBuilder.DropColumn(
                name: "Seq",
                table: "TrackPoints");

            migrationBuilder.AlterColumn<Guid>(
                name: "TrackId",
                table: "TrackSegments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "TrackSegmentId",
                table: "TrackPoints",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackPoints_TrackSegments_TrackSegmentId",
                table: "TrackPoints",
                column: "TrackSegmentId",
                principalTable: "TrackSegments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackSegments_Tracks_TrackId",
                table: "TrackSegments",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
