using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserHistoryChangedToCallRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserHistoryItems");

            migrationBuilder.DropTable(
                name: "UserHistories");

            migrationBuilder.CreateTable(
                name: "CallRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryRef = table.Column<string>(type: "TEXT", nullable: true),
                    PersonType = table.Column<string>(type: "TEXT", nullable: true),
                    PersonId = table.Column<string>(type: "TEXT", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    MobileNo = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    ConcludedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CallRecordItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CallRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    IncomingOutgoing = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfContact = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    GistOfDiscussions = table.Column<string>(type: "TEXT", nullable: false),
                    ContactResult = table.Column<string>(type: "TEXT", nullable: true),
                    NextAction = table.Column<string>(type: "TEXT", nullable: true),
                    NextActionOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ComposeEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    ComposeSMS = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallRecordItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CallRecordItems_CallRecords_CallRecordId",
                        column: x => x.CallRecordId,
                        principalTable: "CallRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CallRecordItems_CallRecordId",
                table: "CallRecordItems",
                column: "CallRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_CallRecords_PersonId",
                table: "CallRecords",
                column: "PersonId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CallRecordItems");

            migrationBuilder.DropTable(
                name: "CallRecords");

            migrationBuilder.CreateTable(
                name: "UserHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryRef = table.Column<string>(type: "TEXT", nullable: true),
                    ConcludedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MobileNo = table.Column<string>(type: "TEXT", nullable: true),
                    PersonName = table.Column<string>(type: "TEXT", nullable: true),
                    ResumeId = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserHistoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComposeEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    ComposeSMS = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContactResult = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfContact = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GistOfDiscussions = table.Column<string>(type: "TEXT", nullable: false),
                    IncomingOutgoing = table.Column<string>(type: "TEXT", nullable: true),
                    NextAction = table.Column<string>(type: "TEXT", nullable: true),
                    NextActionOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    UserHistoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHistoryItems_UserHistories_UserHistoryId",
                        column: x => x.UserHistoryId,
                        principalTable: "UserHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserHistories_CandidateId",
                table: "UserHistories",
                column: "CandidateId",
                unique: true,
                filter: "CandidateId Is Null");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistoryItems_UserHistoryId",
                table: "UserHistoryItems",
                column: "UserHistoryId");
        }
    }
}
