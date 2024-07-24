using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class VoucherCOAtoCoAId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "COAId",
                table: "VoucherEntries",
                newName: "CoaId");

            migrationBuilder.RenameColumn(
                name: "COAId",
                table: "FinanceVouchers",
                newName: "CoaId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DrEntryApprovedOn",
                table: "VoucherEntries",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoaId",
                table: "VoucherEntries",
                newName: "COAId");

            migrationBuilder.RenameColumn(
                name: "CoaId",
                table: "FinanceVouchers",
                newName: "COAId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DrEntryApprovedOn",
                table: "VoucherEntries",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");
        }
    }
}
