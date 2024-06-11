using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class orderassessmentitem_CategoryNameAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfessionId",
                table: "OrderAssessmentItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProfessionName",
                table: "OrderAssessmentItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmigProcessInchargeUsername",
                table: "ContractReviews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicalProcessInchargeUsername",
                table: "ContractReviews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TravelProcessInchargeUsername",
                table: "ContractReviews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisaProcessInchargeUsername",
                table: "ContractReviews",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "OrderAssessmentItems");

            migrationBuilder.DropColumn(
                name: "ProfessionName",
                table: "OrderAssessmentItems");

            migrationBuilder.DropColumn(
                name: "EmigProcessInchargeUsername",
                table: "ContractReviews");

            migrationBuilder.DropColumn(
                name: "MedicalProcessInchargeUsername",
                table: "ContractReviews");

            migrationBuilder.DropColumn(
                name: "TravelProcessInchargeUsername",
                table: "ContractReviews");

            migrationBuilder.DropColumn(
                name: "VisaProcessInchargeUsername",
                table: "ContractReviews");
        }
    }
}
