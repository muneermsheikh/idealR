using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SelDecision_SeletionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_SelectionDecisions_SelectionDecisionId",
                table: "Employments");

            migrationBuilder.DropIndex(
                name: "IX_Employments_SelectionDecisionId",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "SelectionDate",
                table: "SelectionDecisions");

            migrationBuilder.RenameColumn(
                name: "SelectionDecisionId",
                table: "Employments",
                newName: "OfferAccepted");

            migrationBuilder.AlterColumn<string>(
                name: "TaskType",
                table: "Tasks",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CandidateId",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CVRefId",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "SelectionDecisions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProfessionId",
                table: "SelectionDecisions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "SelectionDecisions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FoodNotProvided",
                table: "Employments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HousingNotProvided",
                table: "Employments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SelectionStatus",
                table: "CVRefs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "SelectionStatusDate",
                table: "CVRefs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVRefId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "SelectionDecisions");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "SelectionDecisions");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "SelectionDecisions");

            migrationBuilder.DropColumn(
                name: "FoodNotProvided",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "HousingNotProvided",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "SelectionStatus",
                table: "CVRefs");

            migrationBuilder.DropColumn(
                name: "SelectionStatusDate",
                table: "CVRefs");

            migrationBuilder.RenameColumn(
                name: "OfferAccepted",
                table: "Employments",
                newName: "SelectionDecisionId");

            migrationBuilder.AlterColumn<string>(
                name: "TaskType",
                table: "Tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "Tasks",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "CandidateId",
                table: "Tasks",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<DateTime>(
                name: "SelectionDate",
                table: "SelectionDecisions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Employments_SelectionDecisionId",
                table: "Employments",
                column: "SelectionDecisionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_SelectionDecisions_SelectionDecisionId",
                table: "Employments",
                column: "SelectionDecisionId",
                principalTable: "SelectionDecisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
