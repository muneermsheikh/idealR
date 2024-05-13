using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProcessAndFinanceVoucherOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessItem_Processes_ProcessId",
                table: "ProcessItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessItem",
                table: "ProcessItem");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "FinanceVouchers");

            migrationBuilder.RenameTable(
                name: "ProcessItem",
                newName: "ProcessItems");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessItem_ProcessId",
                table: "ProcessItems",
                newName: "IX_ProcessItems_ProcessId");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "VoucherEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessItems",
                table: "ProcessItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Deps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectedOn = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CurrentStatus = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deps_CVRefs_CVRefId",
                        column: x => x.CVRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Divn = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true),
                    COAId = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", nullable: true),
                    VoucherNo = table.Column<int>(type: "INTEGER", maxLength: 10, nullable: false),
                    VoucherDated = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Amount = table.Column<long>(type: "INTEGER", nullable: false),
                    Narration = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    NextSequence = table.Column<int>(type: "INTEGER", nullable: false),
                    NextSequenceDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DepId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepItems_Deps_DepId",
                        column: x => x.DepId,
                        principalTable: "Deps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FinanceVoucherId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    COAId = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", nullable: true),
                    Dr = table.Column<long>(type: "INTEGER", nullable: false),
                    Cr = table.Column<long>(type: "INTEGER", nullable: false),
                    Narration = table.Column<string>(type: "TEXT", nullable: true),
                    DrEntryApprovedByEmployeeById = table.Column<int>(type: "INTEGER", nullable: false),
                    DrEntryApprovedOn = table.Column<DateOnly>(type: "TEXT", maxLength: 10, nullable: false),
                    DrEntryApproved = table.Column<bool>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    VoucherId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherItems_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepItems_DepId",
                table: "DepItems",
                column: "DepId");

            migrationBuilder.CreateIndex(
                name: "IX_Deps_CVRefId",
                table: "Deps",
                column: "CVRefId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherItems_VoucherId",
                table: "VoucherItems",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessItems_Processes_ProcessId",
                table: "ProcessItems",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessItems_Processes_ProcessId",
                table: "ProcessItems");

            migrationBuilder.DropTable(
                name: "DepItems");

            migrationBuilder.DropTable(
                name: "VoucherItems");

            migrationBuilder.DropTable(
                name: "Deps");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProcessItems",
                table: "ProcessItems");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "VoucherEntries");

            migrationBuilder.RenameTable(
                name: "ProcessItems",
                newName: "ProcessItem");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessItems_ProcessId",
                table: "ProcessItem",
                newName: "IX_ProcessItem_ProcessId");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "FinanceVouchers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProcessItem",
                table: "ProcessItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessItem_Processes_ProcessId",
                table: "ProcessItem",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
