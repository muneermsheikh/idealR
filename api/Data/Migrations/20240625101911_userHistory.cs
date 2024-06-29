using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class userHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerIndustries_CustomerOfficials_CustomerOfficialId",
                table: "CustomerIndustries");

            migrationBuilder.DropIndex(
                name: "IX_CustomerIndustries_CustomerOfficialId",
                table: "CustomerIndustries");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "AlternateEmailId",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "AlternatePhoneNo",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "ApplicationNo",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "CandidateName",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "City",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "Concluded",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "ConcludedByUsername",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "WorkExperience",
                table: "UserHistories");

            migrationBuilder.DropColumn(
                name: "CustomerOfficialId",
                table: "CustomerIndustries");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "UserHistories",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "UserHistories",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "EmailId",
                table: "UserHistories",
                newName: "PersonName");

            migrationBuilder.AddColumn<bool>(
                name: "ComposeEmail",
                table: "UserHistoryItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ComposeSMS",
                table: "UserHistoryItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NextAction",
                table: "UserHistoryItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextActionOn",
                table: "UserHistoryItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StatusDate",
                table: "UserHistories",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "CandidateId",
                table: "UserHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndustryId",
                table: "AgencySpecialties",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "ProspectiveCandidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Gender = table.Column<string>(type: "TEXT", nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 12, nullable: true),
                    DateRegistered = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CategoryRef = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionName = table.Column<string>(type: "TEXT", nullable: true),
                    ResumeId = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true),
                    ResumeTitle = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CandidateName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Age = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    MobileNo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    AlternateNumber = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    AlternateEmail = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentLocation = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    Education = table.Column<string>(type: "TEXT", nullable: false),
                    Ctc = table.Column<string>(type: "TEXT", nullable: true),
                    WorkExperience = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProspectiveCandidates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveCandidates_ResumeId",
                table: "ProspectiveCandidates",
                column: "ResumeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProspectiveCandidates");

            migrationBuilder.DropColumn(
                name: "ComposeEmail",
                table: "UserHistoryItems");

            migrationBuilder.DropColumn(
                name: "ComposeSMS",
                table: "UserHistoryItems");

            migrationBuilder.DropColumn(
                name: "NextAction",
                table: "UserHistoryItems");

            migrationBuilder.DropColumn(
                name: "NextActionOn",
                table: "UserHistoryItems");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "UserHistories",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "UserHistories",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "PersonName",
                table: "UserHistories",
                newName: "EmailId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "StatusDate",
                table: "UserHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CandidateId",
                table: "UserHistories",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Age",
                table: "UserHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlternateEmailId",
                table: "UserHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlternatePhoneNo",
                table: "UserHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationNo",
                table: "UserHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CandidateName",
                table: "UserHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Concluded",
                table: "UserHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ConcludedByUsername",
                table: "UserHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "UserHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkExperience",
                table: "UserHistories",
                type: "TEXT",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerOfficialId",
                table: "CustomerIndustries",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IndustryId",
                table: "AgencySpecialties",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIndustries_CustomerOfficialId",
                table: "CustomerIndustries",
                column: "CustomerOfficialId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerIndustries_CustomerOfficials_CustomerOfficialId",
                table: "CustomerIndustries",
                column: "CustomerOfficialId",
                principalTable: "CustomerOfficials",
                principalColumn: "Id");
        }
    }
}
