using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class FeedbackStddQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Feedbacks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "feedbackStddQs");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Feedbacks");
        }
    }
}
