using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AssessmentBankQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentStddQs");

            migrationBuilder.DropTable(
                name: "CategoryAssessmentQBanks");

            migrationBuilder.DropTable(
                name: "AssessmentQBanks");

            migrationBuilder.CreateTable(
                name: "AssessmentBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentBankQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentBankId = table.Column<int>(type: "int", nullable: false),
                    AssessmentParameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QNo = table.Column<int>(type: "int", nullable: false),
                    IsStandardQ = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentBankQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentBankQs_AssessmentBanks_AssessmentBankId",
                        column: x => x.AssessmentBankId,
                        principalTable: "AssessmentBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentBankQs_AssessmentBankId",
                table: "AssessmentBankQs",
                column: "AssessmentBankId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentBanks_ProfessionId",
                table: "AssessmentBanks",
                column: "ProfessionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentBankQs");

            migrationBuilder.DropTable(
                name: "AssessmentBanks");

            migrationBuilder.CreateTable(
                name: "AssessmentQBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAssessmentQBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentParameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssessmentQBankId = table.Column<int>(type: "int", nullable: false),
                    IsStandardQ = table.Column<bool>(type: "bit", nullable: false),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    QNo = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAssessmentQBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentStddQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentParameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssessmentQBankId = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsStandardQ = table.Column<bool>(type: "bit", nullable: false),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    QNo = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentStddQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentStddQs_AssessmentQBanks_AssessmentQBankId",
                        column: x => x.AssessmentQBankId,
                        principalTable: "AssessmentQBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQBanks_ProfessionId",
                table: "AssessmentQBanks",
                column: "ProfessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentStddQs_AssessmentQBankId",
                table: "AssessmentStddQs",
                column: "AssessmentQBankId");
        }
    }
}
