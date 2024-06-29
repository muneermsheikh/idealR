using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Dep_DestinationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityOfWorking",
                table: "SelectionDecisions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityOfWorking",
                table: "Deps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Deps",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityOfWorking",
                table: "SelectionDecisions");

            migrationBuilder.DropColumn(
                name: "CityOfWorking",
                table: "Deps");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Deps");
        }
    }
}
