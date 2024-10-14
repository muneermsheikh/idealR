using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackDateSent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfficialAppUserId",
                table: "CustomerFeedbacks");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSent",
                table: "CustomerFeedbacks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialUsername",
                table: "CustomerFeedbacks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateSent",
                table: "CustomerFeedbacks");

            migrationBuilder.DropColumn(
                name: "OfficialUsername",
                table: "CustomerFeedbacks");

            migrationBuilder.AddColumn<int>(
                name: "OfficialAppUserId",
                table: "CustomerFeedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
