using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class IntrviewAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntervwItemCandidates_IntervwItems_IntervwItemId",
                table: "IntervwItemCandidates");

            migrationBuilder.DropTable(
                name: "InterviewItems");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.CreateTable(
                name: "AttendanceStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntervwAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntervwItemCandidateId = table.Column<int>(type: "int", nullable: false),
                    AttendanceStatusId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntervwAttendances", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceStatuses_Status",
                table: "AttendanceStatuses",
                column: "Status",
                unique: true,
                filter: "[Status] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceStatuses_StatusId",
                table: "AttendanceStatuses",
                column: "StatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntervwAttendances_IntervwItemCandidateId",
                table: "IntervwAttendances",
                column: "IntervwItemCandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_IntervwAttendances_IntervwItemCandidateId_Status",
                table: "IntervwAttendances",
                columns: new[] { "IntervwItemCandidateId", "Status" },
                unique: true,
                filter: "[Status] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_IntervwItemCandidates_IntervwItems_IntervwItemId",
                table: "IntervwItemCandidates",
                column: "IntervwItemId",
                principalTable: "IntervwItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntervwItemCandidates_IntervwItems_IntervwItemId",
                table: "IntervwItemCandidates");

            migrationBuilder.DropTable(
                name: "AttendanceStatuses");

            migrationBuilder.DropTable(
                name: "IntervwAttendances");

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewDateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewDateUpto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewId = table.Column<int>(type: "int", nullable: false),
                    InterviewMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewVenue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewerRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledUpto = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewItems_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItems_InterviewId",
                table: "InterviewItems",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItems_OrderItemId",
                table: "InterviewItems",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_OrderId",
                table: "Interviews",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IntervwItemCandidates_IntervwItems_IntervwItemId",
                table: "IntervwItemCandidates",
                column: "IntervwItemId",
                principalTable: "IntervwItems",
                principalColumn: "Id");
        }
    }
}
