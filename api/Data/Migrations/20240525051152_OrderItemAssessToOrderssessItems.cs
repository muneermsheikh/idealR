using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemAssessToOrderssessItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderItemAssessments_OrderAssessments_OrderAssessmentId",
                table: "orderItemAssessments");

            migrationBuilder.DropIndex(
                name: "IX_orderItemAssessments_OrderAssessmentId",
                table: "orderItemAssessments");

            migrationBuilder.DropIndex(
                name: "IX_orderItemAssessments_OrderItemId",
                table: "orderItemAssessments");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemAssessmentId_Question",
                table: "OrderItemAssessmentQs");

            migrationBuilder.DropColumn(
                name: "OrderAssessmentId",
                table: "orderItemAssessments");

            migrationBuilder.CreateTable(
                name: "OrderAssessmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    orderAssessmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    RequireCandidateAssessment = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssessmentRef = table.Column<string>(type: "TEXT", nullable: true),
                    DateDesigned = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DesignedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ApprovedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAssessmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAssessmentItems_OrderAssessments_orderAssessmentId",
                        column: x => x.orderAssessmentId,
                        principalTable: "OrderAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderAssessmentItemQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderAssessmentItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Question = table.Column<string>(type: "TEXT", nullable: true),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAssessmentItemQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAssessmentItemQs_OrderAssessmentItems_OrderAssessmentItemId",
                        column: x => x.OrderAssessmentItemId,
                        principalTable: "OrderAssessmentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemAssessmentId",
                table: "OrderItemAssessmentQs",
                column: "OrderItemAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessmentItemQs_OrderAssessmentItemId_Question",
                table: "OrderAssessmentItemQs",
                columns: new[] { "OrderAssessmentItemId", "Question" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessmentItems_orderAssessmentId",
                table: "OrderAssessmentItems",
                column: "orderAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessmentItems_OrderItemId",
                table: "OrderAssessmentItems",
                column: "OrderItemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderAssessmentItemQs");

            migrationBuilder.DropTable(
                name: "OrderAssessmentItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemAssessmentId",
                table: "OrderItemAssessmentQs");

            migrationBuilder.AddColumn<int>(
                name: "OrderAssessmentId",
                table: "orderItemAssessments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_orderItemAssessments_OrderAssessmentId",
                table: "orderItemAssessments",
                column: "OrderAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_orderItemAssessments_OrderItemId",
                table: "orderItemAssessments",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemAssessmentId_Question",
                table: "OrderItemAssessmentQs",
                columns: new[] { "OrderItemAssessmentId", "Question" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orderItemAssessments_OrderAssessments_OrderAssessmentId",
                table: "orderItemAssessments",
                column: "OrderAssessmentId",
                principalTable: "OrderAssessments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
