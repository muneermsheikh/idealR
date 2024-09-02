using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomerIndexFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerName_City",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerName_City",
                table: "Customers",
                columns: new[] { "CustomerName", "City" },
                unique: true,
                filter: "CustomerName IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerName_City",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerName_City",
                table: "Customers",
                columns: new[] { "CustomerName", "City" },
                unique: true);
        }
    }
}
