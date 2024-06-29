using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class CallRecordItem_EmailAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComposeEmail",
                table: "CallRecordItems");

            migrationBuilder.DropColumn(
                name: "ComposeSMS",
                table: "CallRecordItems");

            migrationBuilder.AddColumn<string>(
                name: "PersonName",
                table: "CallRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdvisoryBy",
                table: "CallRecordItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CallRecordItems",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonName",
                table: "CallRecords");

            migrationBuilder.DropColumn(
                name: "AdvisoryBy",
                table: "CallRecordItems");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "CallRecordItems");

            migrationBuilder.AddColumn<bool>(
                name: "ComposeEmail",
                table: "CallRecordItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ComposeSMS",
                table: "CallRecordItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
