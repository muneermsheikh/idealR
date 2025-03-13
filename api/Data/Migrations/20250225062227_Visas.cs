using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Visas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VisaHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisaNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    VisaDateH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaDateG = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    VisaExpiryH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaExpiryG = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisaSponsorName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisaHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisaTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaItemId = table.Column<int>(type: "int", nullable: false),
                    VisaNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepItemId = table.Column<int>(type: "int", nullable: false),
                    VisaAppSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisaApproved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CountUsed = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisaTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisaItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisaHeaderId = table.Column<int>(type: "int", nullable: false),
                    VisaNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaCategoryArabic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaCategoryEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaConsulate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaQuantity = table.Column<int>(type: "int", nullable: false),
                    VisaCanceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisaItems_VisaHeaders_VisaHeaderId",
                        column: x => x.VisaHeaderId,
                        principalTable: "VisaHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisaHeaders_VisaNo",
                table: "VisaHeaders",
                column: "VisaNo",
                unique: true,
                filter: "[VisaNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VisaItems_VisaHeaderId",
                table: "VisaItems",
                column: "VisaHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisaItems");

            migrationBuilder.DropTable(
                name: "VisaTransactions");

            migrationBuilder.DropTable(
                name: "VisaHeaders");
        }
    }
}
