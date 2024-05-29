using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class HelpFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Helps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Topic = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Helps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HelpItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HelpId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    HelpText = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HelpItems_Helps_HelpId",
                        column: x => x.HelpId,
                        principalTable: "Helps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HelpSubItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    helpId = table.Column<int>(type: "INTEGER", nullable: false),
                    sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    SubText = table.Column<string>(type: "TEXT", nullable: true),
                    HelpItemId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpSubItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HelpSubItem_HelpItems_HelpItemId",
                        column: x => x.HelpItemId,
                        principalTable: "HelpItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HelpItems_HelpId_Sequence",
                table: "HelpItems",
                columns: new[] { "HelpId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Helps_Topic",
                table: "Helps",
                column: "Topic",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HelpSubItem_HelpItemId",
                table: "HelpSubItem",
                column: "HelpItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HelpSubItem");

            migrationBuilder.DropTable(
                name: "HelpItems");

            migrationBuilder.DropTable(
                name: "Helps");
        }
    }
}
