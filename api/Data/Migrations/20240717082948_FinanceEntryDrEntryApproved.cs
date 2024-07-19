using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinanceEntryDrEntryApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherAttachments_Vouchers_VoucherId",
                table: "VoucherAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherItems_Vouchers_VoucherId",
                table: "VoucherItems");

            migrationBuilder.DropIndex(
                name: "IX_VoucherAttachments_FileName_VoucherId",
                table: "VoucherAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vouchers",
                table: "Vouchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoucherItems",
                table: "VoucherItems");

            migrationBuilder.DropColumn(
                name: "DrEntryApprovedByEmployeeById",
                table: "VoucherEntries");

            migrationBuilder.RenameTable(
                name: "Vouchers",
                newName: "Voucher");

            migrationBuilder.RenameTable(
                name: "VoucherItems",
                newName: "VoucherItem");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherItems_VoucherId",
                table: "VoucherItem",
                newName: "IX_VoucherItem_VoucherId");

            migrationBuilder.AddColumn<string>(
                name: "DrEntryApprovedByUsername",
                table: "VoucherEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VoucherId",
                table: "VoucherAttachments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "FinanceVoucherId",
                table: "VoucherAttachments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PartyName",
                table: "FinanceVouchers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Voucher",
                table: "Voucher",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoucherItem",
                table: "VoucherItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_FileName_FinanceVoucherId",
                table: "VoucherAttachments",
                columns: new[] { "FileName", "FinanceVoucherId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_FinanceVoucherId",
                table: "VoucherAttachments",
                column: "FinanceVoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherAttachments_FinanceVouchers_FinanceVoucherId",
                table: "VoucherAttachments",
                column: "FinanceVoucherId",
                principalTable: "FinanceVouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherAttachments_Voucher_VoucherId",
                table: "VoucherAttachments",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherItem_Voucher_VoucherId",
                table: "VoucherItem",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherAttachments_FinanceVouchers_FinanceVoucherId",
                table: "VoucherAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherAttachments_Voucher_VoucherId",
                table: "VoucherAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherItem_Voucher_VoucherId",
                table: "VoucherItem");

            migrationBuilder.DropIndex(
                name: "IX_VoucherAttachments_FileName_FinanceVoucherId",
                table: "VoucherAttachments");

            migrationBuilder.DropIndex(
                name: "IX_VoucherAttachments_FinanceVoucherId",
                table: "VoucherAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VoucherItem",
                table: "VoucherItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Voucher",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "DrEntryApprovedByUsername",
                table: "VoucherEntries");

            migrationBuilder.DropColumn(
                name: "FinanceVoucherId",
                table: "VoucherAttachments");

            migrationBuilder.DropColumn(
                name: "PartyName",
                table: "FinanceVouchers");

            migrationBuilder.RenameTable(
                name: "VoucherItem",
                newName: "VoucherItems");

            migrationBuilder.RenameTable(
                name: "Voucher",
                newName: "Vouchers");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherItem_VoucherId",
                table: "VoucherItems",
                newName: "IX_VoucherItems_VoucherId");

            migrationBuilder.AddColumn<int>(
                name: "DrEntryApprovedByEmployeeById",
                table: "VoucherEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "VoucherId",
                table: "VoucherAttachments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoucherItems",
                table: "VoucherItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vouchers",
                table: "Vouchers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_FileName_VoucherId",
                table: "VoucherAttachments",
                columns: new[] { "FileName", "VoucherId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherAttachments_Vouchers_VoucherId",
                table: "VoucherAttachments",
                column: "VoucherId",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherItems_Vouchers_VoucherId",
                table: "VoucherItems",
                column: "VoucherId",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
