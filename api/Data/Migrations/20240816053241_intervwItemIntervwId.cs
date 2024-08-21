using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class intervwItemIntervwId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntervwItems_Intervws_IntervwId",
                table: "IntervwItems");

            migrationBuilder.DropColumn(
                name: "InterviewId",
                table: "IntervwItems");

            migrationBuilder.AlterColumn<int>(
                name: "IntervwId",
                table: "IntervwItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IntervwItems_Intervws_IntervwId",
                table: "IntervwItems",
                column: "IntervwId",
                principalTable: "Intervws",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntervwItems_Intervws_IntervwId",
                table: "IntervwItems");

            migrationBuilder.AlterColumn<int>(
                name: "IntervwId",
                table: "IntervwItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "InterviewId",
                table: "IntervwItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_IntervwItems_Intervws_IntervwId",
                table: "IntervwItems",
                column: "IntervwId",
                principalTable: "Intervws",
                principalColumn: "Id");
        }
    }
}
