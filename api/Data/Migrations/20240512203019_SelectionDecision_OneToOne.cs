using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SelectionDecision_OneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrEntryApprovedByEmployeeById",
                table: "VoucherItems");

            migrationBuilder.DropColumn(
                name: "SelectionDecisionId",
                table: "Employments");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DrEntryApprovedOn",
                table: "VoucherItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "DrEntryApprovedByAppUsername",
                table: "VoucherItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Deps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "Deps",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrEntryApprovedByAppUsername",
                table: "VoucherItems");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Deps");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "Deps");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DrEntryApprovedOn",
                table: "VoucherItems",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DrEntryApprovedByEmployeeById",
                table: "VoucherItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelectionDecisionId",
                table: "Employments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
