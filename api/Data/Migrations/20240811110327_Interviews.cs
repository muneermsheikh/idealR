using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Interviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderAssessmentItems_OrderAssessments_orderAssessmentId",
                table: "OrderAssessmentItems");

            migrationBuilder.DropTable(
                name: "contactResults");

            migrationBuilder.DropTable(
                name: "OrderItemAssessmentQs");

            migrationBuilder.DropTable(
                name: "orderItemAssessments");

            migrationBuilder.DropIndex(
                name: "IX_ProspectiveCandidates_PersonId",
                table: "ProspectiveCandidates");

            migrationBuilder.DropIndex(
                name: "IX_Processes_CVRefId",
                table: "Processes");

            migrationBuilder.DropIndex(
                name: "IX_Employments_SelectionDecisionId",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "ReferredByName",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "CandidateAssessments");

            migrationBuilder.RenameColumn(
                name: "orderAssessmentId",
                table: "OrderAssessmentItems",
                newName: "OrderAssessmentId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAssessmentItems_orderAssessmentId",
                table: "OrderAssessmentItems",
                newName: "IX_OrderAssessmentItems_OrderAssessmentId");

            migrationBuilder.RenameColumn(
                name: "OfferAcceptanceConcludedOn",
                table: "Employments",
                newName: "OfferAcceptedOn");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "ChecklistHRs",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "UserComments",
                table: "ChecklistHRs",
                newName: "HrExecComments");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Candidates",
                newName: "Username");

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "UserExps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SelectionDate",
                table: "SelectionDecisions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "PersonId",
                table: "ProspectiveCandidates",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "ProspectiveCandidates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Education",
                table: "ProspectiveCandidates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistered",
                table: "ProspectiveCandidates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryRef",
                table: "ProspectiveCandidates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(35)",
                oldMaxLength: 35,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ProspectiveCandidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ProspectiveCandidates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumeId",
                table: "ProspectiveCandidates",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "OrderAssessmentItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "OrderAssessmentItemQs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "OfferAccepted",
                table: "Employments",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConclusionStatus",
                table: "Employments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CVReviewId",
                table: "CVRefs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HRExecId",
                table: "CVRefs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ChecklistHRs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "RequireInternalReview",
                table: "CandidateAssessments",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AssessedByEmployeeId",
                table: "CandidateAssessments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewDateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewDateUpto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    InterviewDateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewDateUpto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcludingRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewItems_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewItemCandidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewItemId = table.Column<int>(type: "int", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassportNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduledFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledUpto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InterviewedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttendanceStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcludingRemarkss = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewItemCandidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewItemCandidates_InterviewItems_InterviewItemId",
                        column: x => x.InterviewItemId,
                        principalTable: "InterviewItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveCandidates_PersonId",
                table: "ProspectiveCandidates",
                column: "PersonId",
                unique: true,
                filter: "[PersonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_CVRefId",
                table: "Processes",
                column: "CVRefId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessmentItemQs_OrderItemId",
                table: "OrderAssessmentItemQs",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_SelectionDecisionId",
                table: "Employments",
                column: "SelectionDecisionId",
                unique: true,
                filter: "SelectionDecisionId is NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItemCandidates_InterviewItemId",
                table: "InterviewItemCandidates",
                column: "InterviewItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItems_InterviewId",
                table: "InterviewItems",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewItems_OrderItemId",
                table: "InterviewItems",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_OrderId",
                table: "Interviews",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_SelectionDecisions_SelectionDecisionId",
                table: "Employments",
                column: "SelectionDecisionId",
                principalTable: "SelectionDecisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAssessmentItemQs_OrderItems_OrderItemId",
                table: "OrderAssessmentItemQs",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAssessmentItems_OrderAssessments_OrderAssessmentId",
                table: "OrderAssessmentItems",
                column: "OrderAssessmentId",
                principalTable: "OrderAssessments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_SelectionDecisions_SelectionDecisionId",
                table: "Employments");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAssessmentItemQs_OrderItems_OrderItemId",
                table: "OrderAssessmentItemQs");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderAssessmentItems_OrderAssessments_OrderAssessmentId",
                table: "OrderAssessmentItems");

            migrationBuilder.DropTable(
                name: "InterviewItemCandidates");

            migrationBuilder.DropTable(
                name: "InterviewItems");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_ProspectiveCandidates_PersonId",
                table: "ProspectiveCandidates");

            migrationBuilder.DropIndex(
                name: "IX_Processes_CVRefId",
                table: "Processes");

            migrationBuilder.DropIndex(
                name: "IX_OrderAssessmentItemQs_OrderItemId",
                table: "OrderAssessmentItemQs");

            migrationBuilder.DropIndex(
                name: "IX_Employments_SelectionDecisionId",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "UserExps");

            migrationBuilder.DropColumn(
                name: "SelectionDate",
                table: "SelectionDecisions");

            migrationBuilder.DropColumn(
                name: "City",
                table: "ProspectiveCandidates");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "ProspectiveCandidates");

            migrationBuilder.DropColumn(
                name: "ResumeId",
                table: "ProspectiveCandidates");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OrderAssessmentItems");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "OrderAssessmentItemQs");

            migrationBuilder.DropColumn(
                name: "ConclusionStatus",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "CVReviewId",
                table: "CVRefs");

            migrationBuilder.DropColumn(
                name: "HRExecId",
                table: "CVRefs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChecklistHRs");

            migrationBuilder.DropColumn(
                name: "AssessedByEmployeeId",
                table: "CandidateAssessments");

            migrationBuilder.RenameColumn(
                name: "OrderAssessmentId",
                table: "OrderAssessmentItems",
                newName: "orderAssessmentId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderAssessmentItems_OrderAssessmentId",
                table: "OrderAssessmentItems",
                newName: "IX_OrderAssessmentItems_orderAssessmentId");

            migrationBuilder.RenameColumn(
                name: "OfferAcceptedOn",
                table: "Employments",
                newName: "OfferAcceptanceConcludedOn");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "ChecklistHRs",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "HrExecComments",
                table: "ChecklistHRs",
                newName: "UserComments");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Candidates",
                newName: "UserName");

            migrationBuilder.AlterColumn<string>(
                name: "PersonId",
                table: "ProspectiveCandidates",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "ProspectiveCandidates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Education",
                table: "ProspectiveCandidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateRegistered",
                table: "ProspectiveCandidates",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryRef",
                table: "ProspectiveCandidates",
                type: "nvarchar(35)",
                maxLength: 35,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OfferAccepted",
                table: "Employments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "ReferredByName",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequireInternalReview",
                table: "CandidateAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "CandidateAssessments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "contactResults",
                columns: table => new
                {
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contactResults", x => x.Status);
                });

            migrationBuilder.CreateTable(
                name: "orderItemAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssessmentRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDesigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DesignedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    RequireCandidateAssessment = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderItemAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemAssessmentQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    OrderItemAssessmentId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionNo = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemAssessmentQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemAssessmentQs_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItemAssessmentQs_orderItemAssessments_OrderItemAssessmentId",
                        column: x => x.OrderItemAssessmentId,
                        principalTable: "orderItemAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveCandidates_PersonId",
                table: "ProspectiveCandidates",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Processes_CVRefId",
                table: "Processes",
                column: "CVRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_SelectionDecisionId",
                table: "Employments",
                column: "SelectionDecisionId",
                filter: "SelectionDecisionId is NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_contactResults_Status",
                table: "contactResults",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemAssessmentId",
                table: "OrderItemAssessmentQs",
                column: "OrderItemAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemId",
                table: "OrderItemAssessmentQs",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderAssessmentItems_OrderAssessments_orderAssessmentId",
                table: "OrderAssessmentItems",
                column: "orderAssessmentId",
                principalTable: "OrderAssessments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
