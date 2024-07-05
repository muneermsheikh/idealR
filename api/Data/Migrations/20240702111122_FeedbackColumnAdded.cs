using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prompt1",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prompt2",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prompt3",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prompt4",
                table: "FeedbackItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prompt1",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "Prompt2",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "Prompt3",
                table: "FeedbackItems");

            migrationBuilder.DropColumn(
                name: "Prompt4",
                table: "FeedbackItems");
        }
    }
}
