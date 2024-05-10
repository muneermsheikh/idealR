using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemAssessmentQs_OrderItems_OrderItemId",
                table: "OrderItemAssessmentQs");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "OrderItemAssessmentQs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "OrderItemAssessmentId",
                table: "OrderItemAssessmentQs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "orderItemAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessmentRef = table.Column<string>(type: "TEXT", nullable: true),
                    DateDesigned = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DesignedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ApprovedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderItemAssessments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemAssessmentId_Question",
                table: "OrderItemAssessmentQs",
                columns: new[] { "OrderItemAssessmentId", "Question" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orderItemAssessments_OrderItemId",
                table: "orderItemAssessments",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemAssessmentQs_OrderItems_OrderItemId",
                table: "OrderItemAssessmentQs",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemAssessmentQs_orderItemAssessments_OrderItemAssessmentId",
                table: "OrderItemAssessmentQs",
                column: "OrderItemAssessmentId",
                principalTable: "orderItemAssessments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemAssessmentQs_OrderItems_OrderItemId",
                table: "OrderItemAssessmentQs");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItemAssessmentQs_orderItemAssessments_OrderItemAssessmentId",
                table: "OrderItemAssessmentQs");

            migrationBuilder.DropTable(
                name: "orderItemAssessments");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemAssessmentId_Question",
                table: "OrderItemAssessmentQs");

            migrationBuilder.DropColumn(
                name: "OrderItemAssessmentId",
                table: "OrderItemAssessmentQs");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "OrderItemAssessmentQs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemAssessmentQs_OrderItems_OrderItemId",
                table: "OrderItemAssessmentQs",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
