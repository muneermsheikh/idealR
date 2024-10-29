using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProspectiveRepo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SitePhoneNo",
                table: "IntervwItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteRepName",
                table: "IntervwItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VenueAddress",
                table: "IntervwItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VenueAddress2",
                table: "IntervwItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VenueCityAndPIN",
                table: "IntervwItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProspectiveCandidateId",
                table: "IntervwItemCandidates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SitePhoneNo",
                table: "IntervwItems");

            migrationBuilder.DropColumn(
                name: "SiteRepName",
                table: "IntervwItems");

            migrationBuilder.DropColumn(
                name: "VenueAddress",
                table: "IntervwItems");

            migrationBuilder.DropColumn(
                name: "VenueAddress2",
                table: "IntervwItems");

            migrationBuilder.DropColumn(
                name: "VenueCityAndPIN",
                table: "IntervwItems");

            migrationBuilder.DropColumn(
                name: "ProspectiveCandidateId",
                table: "IntervwItemCandidates");
        }
    }
}
