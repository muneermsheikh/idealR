using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderForwardOfficialsDtFwd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_DateForwarded_CustomerOfficialId",
                table: "OrderForwardCategoryOfficials");

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_CustomerOfficialId",
                table: "OrderForwardCategoryOfficials",
                columns: new[] { "OrderForwardCategoryId", "CustomerOfficialId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_CustomerOfficialId",
                table: "OrderForwardCategoryOfficials");

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_DateForwarded_CustomerOfficialId",
                table: "OrderForwardCategoryOfficials",
                columns: new[] { "OrderForwardCategoryId", "DateForwarded", "CustomerOfficialId" },
                unique: true);
        }
    }
}
