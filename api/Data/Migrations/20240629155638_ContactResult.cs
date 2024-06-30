using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContactResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "CallRecordItems",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "contactResults",
                columns: table => new
                {
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contactResults", x => x.Status);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contactResults_Status",
                table: "contactResults",
                column: "Status",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contactResults");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "CallRecordItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
