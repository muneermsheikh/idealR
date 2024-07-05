using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomerReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerReviewDataId",
                table: "CustomerReviewItems");

            migrationBuilder.RenameColumn(
                name: "ReviewTransactionDate",
                table: "CustomerReviewItems",
                newName: "TransactionDate");

            migrationBuilder.RenameColumn(
                name: "ApprovedBySupUsername",
                table: "CustomerReviewItems",
                newName: "CustomerReviewStatus");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUsername",
                table: "CustomerReviewItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByUsername",
                table: "CustomerReviewItems");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "CustomerReviewItems",
                newName: "ReviewTransactionDate");

            migrationBuilder.RenameColumn(
                name: "CustomerReviewStatus",
                table: "CustomerReviewItems",
                newName: "ApprovedBySupUsername");

            migrationBuilder.AddColumn<int>(
                name: "CustomerReviewDataId",
                table: "CustomerReviewItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
