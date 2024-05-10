using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class contractReviewAndchecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewItemDatas");

            migrationBuilder.DropIndex(
                name: "IX_ContractReviewItems_OrderItemId",
                table: "ContractReviewItems");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistHRs_OrderItemId",
                table: "ChecklistHRs");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "ContractReviews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChecklistHRs");

            migrationBuilder.RenameColumn(
                name: "CategoryName",
                table: "ContractReviewItems",
                newName: "SourceFrom");

            migrationBuilder.RenameColumn(
                name: "HrExecComments",
                table: "ChecklistHRs",
                newName: "UserName");

            migrationBuilder.AddColumn<int>(
                name: "Charges",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByName",
                table: "ContractReviews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfessionName",
                table: "ContractReviewItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserComments",
                table: "ChecklistHRs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContractReviewItemStddQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewParameter = table.Column<string>(type: "TEXT", nullable: true),
                    ResponseText = table.Column<string>(type: "TEXT", nullable: true),
                    ButtonText = table.Column<string>(type: "TEXT", nullable: true),
                    Button2Text = table.Column<string>(type: "TEXT", nullable: true),
                    TextInput = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMandatoryTrue = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractReviewItemStddQs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviewItems_OrderItemId",
                table: "ContractReviewItems",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRs_OrderItemId_CandidateId",
                table: "ChecklistHRs",
                columns: new[] { "OrderItemId", "CandidateId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractReviewItemStddQs");

            migrationBuilder.DropIndex(
                name: "IX_ContractReviewItems_OrderItemId",
                table: "ContractReviewItems");

            migrationBuilder.DropIndex(
                name: "IX_ChecklistHRs_OrderItemId_CandidateId",
                table: "ChecklistHRs");

            migrationBuilder.DropColumn(
                name: "Charges",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ReviewedByName",
                table: "ContractReviews");

            migrationBuilder.DropColumn(
                name: "ProfessionName",
                table: "ContractReviewItems");

            migrationBuilder.DropColumn(
                name: "UserComments",
                table: "ChecklistHRs");

            migrationBuilder.RenameColumn(
                name: "SourceFrom",
                table: "ContractReviewItems",
                newName: "CategoryName");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "ChecklistHRs",
                newName: "HrExecComments");

            migrationBuilder.AddColumn<int>(
                name: "ReviewedBy",
                table: "ContractReviews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ChecklistHRs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReviewItemDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsMandatoryTrue = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsResponseBoolean = table.Column<bool>(type: "INTEGER", nullable: false),
                    Response = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReviewParameter = table.Column<string>(type: "TEXT", nullable: true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewItemDatas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviewItems_OrderItemId",
                table: "ContractReviewItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRs_OrderItemId",
                table: "ChecklistHRs",
                column: "OrderItemId");
        }
    }
}
