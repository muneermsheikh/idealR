using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Professions_ProfessionId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Charges",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "FeeFromClientINR",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ReviewItemStatusId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ProfessionName",
                table: "OrderItems",
                newName: "ReviewItemStatus");

            migrationBuilder.AlterColumn<int>(
                name: "ProfessionId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Professions_ProfessionId",
                table: "OrderItems",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Professions_ProfessionId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ReviewItemStatus",
                table: "OrderItems",
                newName: "ProfessionName");

            migrationBuilder.AlterColumn<int>(
                name: "ProfessionId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Charges",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FeeFromClientINR",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewItemStatusId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Professions_ProfessionId",
                table: "OrderItems",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id");
        }
    }
}
