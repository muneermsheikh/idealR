using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProspectivePersonId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProspectiveCandidates_ResumeId",
                table: "ProspectiveCandidates");

            migrationBuilder.RenameColumn(
                name: "MobileNo",
                table: "ProspectiveCandidates",
                newName: "PhoneNo");

            migrationBuilder.RenameColumn(
                name: "MobileNo",
                table: "CallRecords",
                newName: "PhoneNo");

            migrationBuilder.AlterColumn<string>(
                name: "ResumeId",
                table: "ProspectiveCandidates",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 15);

            migrationBuilder.AddColumn<string>(
                name: "PersonId",
                table: "ProspectiveCandidates",
                type: "TEXT",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "ProspectiveCandidates");

            migrationBuilder.RenameColumn(
                name: "PhoneNo",
                table: "ProspectiveCandidates",
                newName: "MobileNo");

            migrationBuilder.RenameColumn(
                name: "PhoneNo",
                table: "CallRecords",
                newName: "MobileNo");

            migrationBuilder.AlterColumn<string>(
                name: "ResumeId",
                table: "ProspectiveCandidates",
                type: "TEXT",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveCandidates_ResumeId",
                table: "ProspectiveCandidates",
                column: "ResumeId",
                unique: true);
        }
    }
}
