using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class interviewItemCandidateRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewItemCandidates");

            migrationBuilder.RenameColumn(
                name: "InterviewDateUpto",
                table: "InterviewItems",
                newName: "ScheduledUpto");

            migrationBuilder.RenameColumn(
                name: "InterviewDateFrom",
                table: "InterviewItems",
                newName: "ScheduledFrom");

            migrationBuilder.RenameColumn(
                name: "ConcludingRemarks",
                table: "InterviewItems",
                newName: "InterviewerRemarks");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationNo",
                table: "InterviewItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CandidateId",
                table: "InterviewItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CandidateName",
                table: "InterviewItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InterviewedAt",
                table: "InterviewItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportedAt",
                table: "InterviewItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationNo",
                table: "InterviewItems");

            migrationBuilder.DropColumn(
                name: "CandidateId",
                table: "InterviewItems");

            migrationBuilder.DropColumn(
                name: "CandidateName",
                table: "InterviewItems");

            migrationBuilder.DropColumn(
                name: "InterviewedAt",
                table: "InterviewItems");

            migrationBuilder.DropColumn(
                name: "ReportedAt",
                table: "InterviewItems");

            migrationBuilder.RenameColumn(
                name: "ScheduledUpto",
                table: "InterviewItems",
                newName: "InterviewDateUpto");

            migrationBuilder.RenameColumn(
                name: "ScheduledFrom",
                table: "InterviewItems",
                newName: "InterviewDateFrom");

            migrationBuilder.RenameColumn(
                name: "InterviewerRemarks",
                table: "InterviewItems",
                newName: "ConcludingRemarks");

            migrationBuilder.CreateTable(
                name: "InterviewItemCandidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    AttendanceStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcludingRemarkss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewItemId = table.Column<int>(type: "int", nullable: false),
                    InterviewMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassportNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledUpto = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewItemCandidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewItemCandidates_InterviewItems_InterviewItemId",
                        column: x => x.InterviewItemId,
                        principalTable: "InterviewItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItemCandidates_InterviewItemId",
                table: "InterviewItemCandidates",
                column: "InterviewItemId");
        }
    }
}
