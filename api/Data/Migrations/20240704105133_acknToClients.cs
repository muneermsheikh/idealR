using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class acknToClients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AckanowledgeToClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateAcknowledged = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecipientUsername = table.Column<string>(type: "TEXT", nullable: true),
                    RecipientEmailId = table.Column<string>(type: "TEXT", nullable: true),
                    SenderUsername = table.Column<string>(type: "TEXT", nullable: true),
                    SenderEmailId = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    MessageType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AckanowledgeToClients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AckanowledgeToClients_OrderId",
                table: "AckanowledgeToClients",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AckanowledgeToClients");
        }
    }
}
