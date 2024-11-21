using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class HRtasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherAttachments_Voucher_VoucherId",
                table: "VoucherAttachments");

            migrationBuilder.DropTable(
                name: "VoucherItem");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropIndex(
                name: "IX_VoucherAttachments_VoucherId",
                table: "VoucherAttachments");

            migrationBuilder.DropColumn(
                name: "VoucherId",
                table: "VoucherAttachments");

            migrationBuilder.AddColumn<int>(
                name: "QntyAssigned",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QntyDelivered",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HRTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedByUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedToUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    QntyAssigned = table.Column<int>(type: "int", nullable: false),
                    QntyDelivered = table.Column<int>(type: "int", nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompleteBy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskStatusDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRTaskItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HRTaskId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HRExecutiveUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    CandidateAssessmentId = table.Column<int>(type: "int", nullable: false),
                    CVRefId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRTaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HRTaskItems_HRTasks_HRTaskId",
                        column: x => x.HRTaskId,
                        principalTable: "HRTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HRTaskItems_CandidateId_HRTaskId",
                table: "HRTaskItems",
                columns: new[] { "CandidateId", "HRTaskId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HRTaskItems_HRTaskId",
                table: "HRTaskItems",
                column: "HRTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_HRTasks_OrderItemId_AssignedToUsername",
                table: "HRTasks",
                columns: new[] { "OrderItemId", "AssignedToUsername" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HRTaskItems");

            migrationBuilder.DropTable(
                name: "HRTasks");

            migrationBuilder.DropColumn(
                name: "QntyAssigned",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "QntyDelivered",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "VoucherId",
                table: "VoucherAttachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    COAId = table.Column<int>(type: "int", nullable: false),
                    CVRefId = table.Column<int>(type: "int", nullable: false),
                    Divn = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Narration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoucherDated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VoucherNo = table.Column<int>(type: "int", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COAId = table.Column<int>(type: "int", nullable: false),
                    Cr = table.Column<long>(type: "bigint", nullable: false),
                    Dr = table.Column<long>(type: "bigint", nullable: false),
                    DrEntryApproved = table.Column<bool>(type: "bit", nullable: false),
                    DrEntryApprovedByAppUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrEntryApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Narration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VoucherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherItem_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_VoucherId",
                table: "VoucherAttachments",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherItem_VoucherId",
                table: "VoucherItem",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherAttachments_Voucher_VoucherId",
                table: "VoucherAttachments",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id");
        }
    }
}
