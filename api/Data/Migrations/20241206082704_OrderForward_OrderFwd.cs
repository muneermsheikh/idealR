using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderForward_OrderFwd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderForwardToAgentId",
                table: "OrderForwardCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardCategories_OrderForwardToAgentId",
                table: "OrderForwardCategories",
                column: "OrderForwardToAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderForwardCategories_OrderForwardToAgents_OrderForwardToAgentId",
                table: "OrderForwardCategories",
                column: "OrderForwardToAgentId",
                principalTable: "OrderForwardToAgents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderForwardCategories_OrderForwardToAgents_OrderForwardToAgentId",
                table: "OrderForwardCategories");

            migrationBuilder.DropIndex(
                name: "IX_OrderForwardCategories_OrderForwardToAgentId",
                table: "OrderForwardCategories");

            migrationBuilder.DropColumn(
                name: "OrderForwardToAgentId",
                table: "OrderForwardCategories");
        }
    }
}
