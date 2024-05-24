using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApiFinalized : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DLForwardedToAgents");

            migrationBuilder.DropColumn(
                name: "HRExecId",
                table: "CVRefs");

            migrationBuilder.DropColumn(
                name: "HRExecId",
                table: "ChecklistHRs");

            migrationBuilder.AddColumn<int>(
                name: "OrderAssessmentId",
                table: "orderItemAssessments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SelectionDecisionId",
                table: "Employments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TransportNotProvided",
                table: "Employments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HRExecUsername",
                table: "CVRefs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlackListed",
                table: "Customers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HRExecUsername",
                table: "ChecklistHRs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AadharNo",
                table: "Candidates",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationDesired",
                table: "Candidates",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AssessmentQBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProfessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DesignedByUsername = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderForwardToAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    customerName = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerCity = table.Column<string>(type: "TEXT", nullable: true),
                    ProjectManagerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderForwardToAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderForwardToAgents_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderForwardToHRs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecipientUsername = table.Column<string>(type: "TEXT", nullable: true),
                    DateOnlyForwarded = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderForwardToHRs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QualificationName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    AppUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AttachmentType = table.Column<string>(type: "TEXT", nullable: true),
                    Length = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    UploadedLocation = table.Column<string>(type: "TEXT", nullable: true),
                    UploadedbyUserName = table.Column<string>(type: "TEXT", nullable: true),
                    UploadedOn = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAttachments_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryRef = table.Column<string>(type: "TEXT", nullable: true),
                    Gender = table.Column<string>(type: "TEXT", nullable: true),
                    Age = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResumeId = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    CandidateName = table.Column<string>(type: "TEXT", nullable: true),
                    EmailId = table.Column<string>(type: "TEXT", nullable: true),
                    AlternateEmailId = table.Column<string>(type: "TEXT", nullable: true),
                    MobileNo = table.Column<string>(type: "TEXT", nullable: true),
                    AlternatePhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    Education = table.Column<string>(type: "TEXT", nullable: true),
                    WorkExperience = table.Column<string>(type: "TEXT", maxLength: 25, nullable: true),
                    CreatedOn = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Concluded = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    StatusDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    ConcludedOn = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    ConcludedByUsername = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VoucherId = table.Column<int>(type: "INTEGER", nullable: false),
                    AttachmentSizeInBytes = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    DateUploaded = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    UploadedByUsername = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherAttachments_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentStddQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssessmentQBankId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessmentParameter = table.Column<string>(type: "TEXT", nullable: true),
                    QNo = table.Column<int>(type: "INTEGER", nullable: false),
                    IsStandardQ = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false),
                    Question = table.Column<string>(type: "TEXT", nullable: true),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CustomerReviewItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerReviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewTransactionDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CustomerReviewDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    ApprovedBySupUsername = table.Column<string>(type: "TEXT", nullable: true),
                    ApprovedOn = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReviewItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReviewItems_CustomerReviews_CustomerReviewId",
                        column: x => x.CustomerReviewId,
                        principalTable: "CustomerReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderForwardCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderForwardToAgentId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionName = table.Column<string>(type: "TEXT", nullable: true),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderForwardCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderForwardCategories_OrderForwardToAgents_OrderForwardToAgentId",
                        column: x => x.OrderForwardToAgentId,
                        principalTable: "OrderForwardToAgents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHistoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserHistoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    IncomingOutgoing = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfContact = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    GistOfDiscussions = table.Column<string>(type: "TEXT", nullable: false),
                    ContactResult = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHistoryItems_UserHistories_UserHistoryId",
                        column: x => x.UserHistoryId,
                        principalTable: "UserHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderForwardCategoryOfficials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderForwardCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerOfficialId = table.Column<int>(type: "INTEGER", nullable: false),
                    AgentName = table.Column<string>(type: "TEXT", nullable: true),
                    DateOnlyForwarded = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EmailIdForwardedTo = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNoForwardedTo = table.Column<string>(type: "TEXT", nullable: true),
                    WhatsAppNoForwardedTo = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderForwardCategoryOfficials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderForwardCategoryOfficials_OrderForwardCategories_OrderForwardCategoryId",
                        column: x => x.OrderForwardCategoryId,
                        principalTable: "OrderForwardCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orderItemAssessments_OrderAssessmentId",
                table: "orderItemAssessments",
                column: "OrderAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Industries_IndustryName",
                table: "Industries",
                column: "IndustryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQBanks_ProfessionId",
                table: "AssessmentQBanks",
                column: "ProfessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentStddQs_AssessmentQBankId",
                table: "AssessmentStddQs",
                column: "AssessmentQBankId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviewItems_CustomerReviewId",
                table: "CustomerReviewItems",
                column: "CustomerReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReviews_CustomerId",
                table: "CustomerReviews",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessments_OrderId",
                table: "OrderAssessments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardCategories_OrderForwardToAgentId",
                table: "OrderForwardCategories",
                column: "OrderForwardToAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardCategories_OrderItemId",
                table: "OrderForwardCategories",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_DateOnlyForwarded_CustomerOfficialId",
                table: "OrderForwardCategoryOfficials",
                columns: new[] { "OrderForwardCategoryId", "DateOnlyForwarded", "CustomerOfficialId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardToAgents_OrderId",
                table: "OrderForwardToAgents",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardToHRs_OrderId_DateOnlyForwarded",
                table: "OrderForwardToHRs",
                columns: new[] { "OrderId", "DateOnlyForwarded" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_QualificationName",
                table: "Qualifications",
                column: "QualificationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAttachments_CandidateId",
                table: "UserAttachments",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistories_CandidateId",
                table: "UserHistories",
                column: "CandidateId",
                unique: true,
                filter: "CandidateId Is Null");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistoryItems_UserHistoryId",
                table: "UserHistoryItems",
                column: "UserHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_FileName_VoucherId",
                table: "VoucherAttachments",
                columns: new[] { "FileName", "VoucherId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_VoucherId",
                table: "VoucherAttachments",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_orderItemAssessments_OrderAssessments_OrderAssessmentId",
                table: "orderItemAssessments",
                column: "OrderAssessmentId",
                principalTable: "OrderAssessments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderItemAssessments_OrderAssessments_OrderAssessmentId",
                table: "orderItemAssessments");

            migrationBuilder.DropTable(
                name: "AssessmentStddQs");

            migrationBuilder.DropTable(
                name: "CustomerReviewItems");

            migrationBuilder.DropTable(
                name: "OrderAssessments");

            migrationBuilder.DropTable(
                name: "OrderForwardCategoryOfficials");

            migrationBuilder.DropTable(
                name: "OrderForwardToHRs");

            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropTable(
                name: "UserAttachments");

            migrationBuilder.DropTable(
                name: "UserHistoryItems");

            migrationBuilder.DropTable(
                name: "VoucherAttachments");

            migrationBuilder.DropTable(
                name: "AssessmentQBanks");

            migrationBuilder.DropTable(
                name: "CustomerReviews");

            migrationBuilder.DropTable(
                name: "OrderForwardCategories");

            migrationBuilder.DropTable(
                name: "UserHistories");

            migrationBuilder.DropTable(
                name: "OrderForwardToAgents");

            migrationBuilder.DropIndex(
                name: "IX_orderItemAssessments_OrderAssessmentId",
                table: "orderItemAssessments");

            migrationBuilder.DropIndex(
                name: "IX_Industries_IndustryName",
                table: "Industries");

            migrationBuilder.DropColumn(
                name: "OrderAssessmentId",
                table: "orderItemAssessments");

            migrationBuilder.DropColumn(
                name: "TransportNotProvided",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "HRExecUsername",
                table: "CVRefs");

            migrationBuilder.DropColumn(
                name: "IsBlackListed",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "HRExecUsername",
                table: "ChecklistHRs");

            migrationBuilder.DropColumn(
                name: "AadharNo",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "NotificationDesired",
                table: "Candidates");

            migrationBuilder.AlterColumn<int>(
                name: "SelectionDecisionId",
                table: "Employments",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "HRExecId",
                table: "CVRefs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HRExecId",
                table: "ChecklistHRs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DLForwardedToAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerOfficialId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateForwarded = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DLForwardedToAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DLForwardedToAgents_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DLForwardedToAgents_CustomerOfficialId_DateForwarded_OrderItemId",
                table: "DLForwardedToAgents",
                columns: new[] { "CustomerOfficialId", "DateForwarded", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DLForwardedToAgents_OrderId",
                table: "DLForwardedToAgents",
                column: "OrderId");
        }
    }
}
