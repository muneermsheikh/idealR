using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Help : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpSubItem_HelpItems_HelpItemId",
                table: "HelpSubItem");

            migrationBuilder.DropColumn(
                name: "helpId",
                table: "HelpSubItem");

            migrationBuilder.RenameColumn(
                name: "sequence",
                table: "HelpSubItem",
                newName: "Sequence");

            migrationBuilder.RenameColumn(
                name: "SubText",
                table: "HelpSubItem",
                newName: "HelpText");

            migrationBuilder.RenameColumn(
                name: "HelpText",
                table: "HelpItems",
                newName: "HelpSubTopic");

            migrationBuilder.AlterColumn<int>(
                name: "HelpItemId",
                table: "HelpSubItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HelpSubItem_HelpItems_HelpItemId",
                table: "HelpSubItem",
                column: "HelpItemId",
                principalTable: "HelpItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpSubItem_HelpItems_HelpItemId",
                table: "HelpSubItem");

            migrationBuilder.RenameColumn(
                name: "Sequence",
                table: "HelpSubItem",
                newName: "sequence");

            migrationBuilder.RenameColumn(
                name: "HelpText",
                table: "HelpSubItem",
                newName: "SubText");

            migrationBuilder.RenameColumn(
                name: "HelpSubTopic",
                table: "HelpItems",
                newName: "HelpText");

            migrationBuilder.AlterColumn<int>(
                name: "HelpItemId",
                table: "HelpSubItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "helpId",
                table: "HelpSubItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_HelpSubItem_HelpItems_HelpItemId",
                table: "HelpSubItem",
                column: "HelpItemId",
                principalTable: "HelpItems",
                principalColumn: "Id");
        }
    }
}
