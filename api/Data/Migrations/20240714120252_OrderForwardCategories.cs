using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderForwardCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderForwardCategories_OrderForwardToAgents_OrderForwardToAgentId",
                table: "OrderForwardCategories");

            migrationBuilder.DropIndex(
                name: "IX_OrderForwardCategories_OrderForwardToAgentId",
                table: "OrderForwardCategories");

            migrationBuilder.RenameColumn(
                name: "OrderForwardToAgentId",
                table: "OrderForwardCategories",
                newName: "OrderNo");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "OrderForwardCategoryOfficials",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCity",
                table: "OrderForwardCategories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "OrderForwardCategories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "OrderForwardCategories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "OrderForwardCategoryOfficials");

            migrationBuilder.DropColumn(
                name: "CustomerCity",
                table: "OrderForwardCategories");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "OrderForwardCategories");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "OrderForwardCategories");

            migrationBuilder.RenameColumn(
                name: "OrderNo",
                table: "OrderForwardCategories",
                newName: "OrderForwardToAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardCategories_OrderForwardToAgentId",
                table: "OrderForwardCategories",
                column: "OrderForwardToAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderForwardCategories_OrderForwardToAgents_OrderForwardToAgentId",
                table: "OrderForwardCategories",
                column: "OrderForwardToAgentId",
                principalTable: "OrderForwardToAgents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
