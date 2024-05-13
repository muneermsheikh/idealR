using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Employment_ConcludedFieldsChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OfferAcceptedOn",
                table: "Employments",
                newName: "OfferAcceptanceConcludedOn");

            migrationBuilder.RenameColumn(
                name: "OfferAcceptedByUsername",
                table: "Employments",
                newName: "OfferConclusionRegisteredByUsername");

            migrationBuilder.AlterColumn<string>(
                name: "OfferAccepted",
                table: "Employments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OfferConclusionRegisteredByUsername",
                table: "Employments",
                newName: "OfferAcceptedByUsername");

            migrationBuilder.RenameColumn(
                name: "OfferAcceptanceConcludedOn",
                table: "Employments",
                newName: "OfferAcceptedOn");

            migrationBuilder.AlterColumn<bool>(
                name: "OfferAccepted",
                table: "Employments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
