using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackItems_Feedbacks_FeedbackId",
                table: "FeedbackItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLike_AspNetUsers_TargetUserId",
                table: "UserLike");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "feedbackStddQs");

            migrationBuilder.DropIndex(
                name: "IX_FeedbackItems_FeedbackId",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "FeedbackQNo",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "FeedbackQuestion",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "TextForLevel1",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "TextForLevel2",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "TextForLevel3",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "TextForLevel4",
                table: "FeedbackItems");

            migrationBuilder.RenameColumn(
                name: "IsMandatory",
                table: "FeedbackItems",
                newName: "QuestionNo");

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "FeedbackItems",
                type: "TEXT",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerFeedbackId",
                table: "FeedbackItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "FeedbackItems",
                type: "TEXT",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PriorityAccount",
                table: "CustomerOfficials",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityAdmin",
                table: "CustomerOfficials",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityHR",
                table: "CustomerOfficials",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CustomerFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    OfficialName = table.Column<string>(type: "TEXT", nullable: true),
                    Designation = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    DateIssued = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HowReceived = table.Column<string>(type: "TEXT", nullable: true),
                    GradeAssessedByClient = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerSuggestion = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FeedbackGroup = table.Column<string>(type: "TEXT", nullable: true),
                    QuestionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Question = table.Column<string>(type: "TEXT", nullable: true),
                    Prompt1 = table.Column<string>(type: "TEXT", nullable: true),
                    Prompt2 = table.Column<string>(type: "TEXT", nullable: true),
                    Prompt3 = table.Column<string>(type: "TEXT", nullable: true),
                    Prompt4 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackQs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackItems_CustomerFeedbackId",
                table: "FeedbackItems",
                column: "CustomerFeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackItems_CustomerFeedbacks_CustomerFeedbackId",
                table: "FeedbackItems",
                column: "CustomerFeedbackId",
                principalTable: "CustomerFeedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLike_AspNetUsers_TargetUserId",
                table: "UserLike",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackItems_CustomerFeedbacks_CustomerFeedbackId",
                table: "FeedbackItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLike_AspNetUsers_TargetUserId",
                table: "UserLike");

            migrationBuilder.DropTable(
                name: "CustomerFeedbacks");

            migrationBuilder.DropTable(
                name: "FeedbackQs");

            migrationBuilder.DropIndex(
                name: "IX_FeedbackItems_CustomerFeedbackId",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "CustomerFeedbackId",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "Question",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "PriorityAccount",
                table: "CustomerOfficials");

            migrationBuilder.DropColumn(
                name: "PriorityAdmin",
                table: "CustomerOfficials");

            migrationBuilder.DropColumn(
                name: "PriorityHR",
                table: "CustomerOfficials");

            migrationBuilder.RenameColumn(
                name: "QuestionNo",
                table: "FeedbackItems",
                newName: "IsMandatory");

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 15);

            migrationBuilder.AddColumn<int>(
                name: "FeedbackQNo",
                table: "FeedbackItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FeedbackQuestion",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextForLevel1",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextForLevel2",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextForLevel3",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextForLevel4",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    HowReceived = table.Column<string>(type: "TEXT", nullable: true),
                    IssuedOn = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ReceivedOn = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "feedbackStddQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FeedbackGroup = table.Column<string>(type: "TEXT", nullable: true),
                    FeedbackQNo = table.Column<int>(type: "INTEGER", nullable: false),
                    FeedbackQuestion = table.Column<string>(type: "TEXT", nullable: true),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false),
                    TextForLevel1 = table.Column<string>(type: "TEXT", nullable: true),
                    TextForLevel2 = table.Column<string>(type: "TEXT", nullable: true),
                    TextForLevel3 = table.Column<string>(type: "TEXT", nullable: true),
                    TextForLevel4 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedbackStddQs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackItems_FeedbackId",
                table: "FeedbackItems",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackItems_Feedbacks_FeedbackId",
                table: "FeedbackItems",
                column: "FeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLike_AspNetUsers_TargetUserId",
                table: "UserLike",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
