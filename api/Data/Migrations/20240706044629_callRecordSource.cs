using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class callRecordSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "CustomerFeedbacks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfficialAppUserId",
                table: "CustomerFeedbacks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "CallRecords",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "CustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "OfficialAppUserId",
                table: "CustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "CallRecords");
        }
    }
}
