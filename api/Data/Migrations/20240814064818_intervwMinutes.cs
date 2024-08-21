using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class intervwMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledUpto",
                table: "IntervwItemCandidates");

            migrationBuilder.AddColumn<int>(
                name: "EstimatedMinsToInterviewEachCandidate",
                table: "IntervwItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedMinsToInterviewEachCandidate",
                table: "IntervwItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledUpto",
                table: "IntervwItemCandidates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
