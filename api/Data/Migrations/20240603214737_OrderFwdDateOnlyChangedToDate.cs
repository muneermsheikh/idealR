using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderFwdDateOnlyChangedToDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Charges",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "DateOnlyForwarded",
                table: "OrderForwardCategoryOfficials",
                newName: "DateForwarded");

            migrationBuilder.RenameIndex(
                name: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_DateOnlyForwarded_CustomerOfficialId",
                table: "OrderForwardCategoryOfficials",
                newName: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_DateForwarded_CustomerOfficialId");

            migrationBuilder.RenameColumn(
                name: "HRExecUsername",
                table: "ContractReviewItems",
                newName: "HrExecUsername");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateForwarded",
                table: "OrderForwardCategoryOfficials",
                newName: "DateOnlyForwarded");

            migrationBuilder.RenameIndex(
                name: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_DateForwarded_CustomerOfficialId",
                table: "OrderForwardCategoryOfficials",
                newName: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_DateOnlyForwarded_CustomerOfficialId");

            migrationBuilder.RenameColumn(
                name: "HrExecUsername",
                table: "ContractReviewItems",
                newName: "HRExecUsername");

            migrationBuilder.AddColumn<int>(
                name: "Charges",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
