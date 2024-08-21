using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Intervw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Intervws",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewDateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewDateUpto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intervws", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntervwItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewId = table.Column<int>(type: "int", nullable: false),
                    InterviewVenue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntervwId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntervwItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntervwItems_Intervws_IntervwId",
                        column: x => x.IntervwId,
                        principalTable: "Intervws",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IntervwItemCandidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewItemId = table.Column<int>(type: "int", nullable: false),
                    ScheduledFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledUpto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewerRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntervwItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntervwItemCandidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntervwItemCandidates_IntervwItems_IntervwItemId",
                        column: x => x.IntervwItemId,
                        principalTable: "IntervwItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntervwItemCandidates_CandidateId_InterviewItemId",
                table: "IntervwItemCandidates",
                columns: new[] { "CandidateId", "InterviewItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntervwItemCandidates_IntervwItemId",
                table: "IntervwItemCandidates",
                column: "IntervwItemId");

            migrationBuilder.CreateIndex(
                name: "IX_IntervwItems_IntervwId",
                table: "IntervwItems",
                column: "IntervwId");

            migrationBuilder.CreateIndex(
                name: "IX_IntervwItems_OrderItemId",
                table: "IntervwItems",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Intervws_OrderId",
                table: "Intervws",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntervwItemCandidates");

            migrationBuilder.DropTable(
                name: "IntervwItems");

            migrationBuilder.DropTable(
                name: "Intervws");
        }
    }
}
