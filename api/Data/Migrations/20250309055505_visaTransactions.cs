using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class visaTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "VisaTransactions");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "VisaTransactions");

            migrationBuilder.RenameColumn(
                name: "CountUsed",
                table: "VisaTransactions",
                newName: "CustomerId");

            migrationBuilder.AddColumn<DateTime>(
                name: "VisaDateG",
                table: "VisaTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisaDateG",
                table: "VisaTransactions");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "VisaTransactions",
                newName: "CountUsed");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "VisaTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "VisaTransactions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
