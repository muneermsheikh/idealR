using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdealEntitiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_SourceUserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_TargetUserId",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.RenameTable(
                name: "Likes",
                newName: "UserLike");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_TargetUserId",
                table: "UserLike",
                newName: "IX_UserLike_TargetUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLike",
                table: "UserLike",
                columns: new[] { "SourceUserId", "TargetUserId" });

            migrationBuilder.CreateTable(
                name: "AssessmentQStdds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Question = table.Column<string>(type: "TEXT", nullable: true),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQStdds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAssessmentQBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssessmentQBankId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessmentParameter = table.Column<string>(type: "TEXT", nullable: true),
                    QNo = table.Column<int>(type: "INTEGER", nullable: false),
                    IsStandardQ = table.Column<bool>(type: "INTEGER", nullable: false),
                    Question = table.Column<string>(type: "TEXT", nullable: true),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAssessmentQBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistHRDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Parameter = table.Column<string>(type: "TEXT", nullable: true),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistHRDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COAs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Divn = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    AccountType = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AccountClass = table.Column<string>(type: "TEXT", nullable: true),
                    OpBalance = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COAs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerType = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    KnownAs = table.Column<string>(type: "TEXT", nullable: true),
                    Add = table.Column<string>(type: "TEXT", nullable: true),
                    Add2 = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Pin = table.Column<string>(type: "TEXT", nullable: true),
                    District = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Phone2 = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Introduction = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerStatus = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CVRefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVReviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferredOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HRExecId = table.Column<int>(type: "INTEGER", nullable: false),
                    RefStatus = table.Column<string>(type: "TEXT", nullable: true),
                    RefStatusDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVRefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeployStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StatusName = table.Column<string>(type: "TEXT", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    NextSequence = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkingDaysReqdForNextStage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeployStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: true),
                    Gender = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    SecondName = table.Column<string>(type: "TEXT", nullable: true),
                    FamilyName = table.Column<string>(type: "TEXT", nullable: true),
                    KnownAs = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<string>(type: "TEXT", nullable: false),
                    Qualifications = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlaceOfBirth = table.Column<string>(type: "TEXT", nullable: true),
                    AadharNo = table.Column<string>(type: "TEXT", nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    EmployeeAddress = table.Column<string>(type: "TEXT", nullable: true),
                    EmployeePhone = table.Column<string>(type: "TEXT", nullable: true),
                    EmployeePhone2 = table.Column<string>(type: "TEXT", nullable: true),
                    EmployeeQualifications = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    Address2 = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    IssuedOn = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ReceivedOn = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    HowReceived = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinanceVouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Divn = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true),
                    COAId = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", nullable: true),
                    VoucherNo = table.Column<int>(type: "INTEGER", maxLength: 10, nullable: false),
                    VoucherDated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<long>(type: "INTEGER", nullable: false),
                    Narration = table.Column<string>(type: "TEXT", nullable: true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceVouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Industries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IndustryName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Industries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProfessionName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewItemDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewParameter = table.Column<string>(type: "TEXT", nullable: true),
                    IsResponseBoolean = table.Column<bool>(type: "INTEGER", nullable: false),
                    Response = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMandatoryTrue = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewItemDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SkillName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserQualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    QualificationId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQualifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgencySpecialties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    IndustryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencySpecialties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgencySpecialties_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOfficials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    OfficialName = table.Column<string>(type: "TEXT", nullable: true),
                    KnownAs = table.Column<string>(type: "TEXT", nullable: true),
                    Designation = table.Column<string>(type: "TEXT", nullable: true),
                    Divn = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNo = table.Column<string>(type: "TEXT", nullable: true),
                    Mobile = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOfficials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOfficials_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderRef = table.Column<string>(type: "TEXT", nullable: true),
                    OrderRefDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProjectManagerId = table.Column<int>(type: "INTEGER", nullable: false),
                    SalesmanId = table.Column<int>(type: "INTEGER", nullable: true),
                    CompleteBy = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    CityOfWorking = table.Column<string>(type: "TEXT", nullable: true),
                    ContractReviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractReviewStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    ForwardedToHRDeptOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    SecondName = table.Column<string>(type: "TEXT", nullable: true),
                    FamilyName = table.Column<string>(type: "TEXT", nullable: true),
                    KnownAs = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: true),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    DOB = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PpNo = table.Column<string>(type: "TEXT", nullable: true),
                    Ecnr = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Pin = table.Column<string>(type: "TEXT", nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastActive = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AppUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Qualifications = table.Column<string>(type: "TEXT", nullable: true),
                    PhotoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidates_CVRefs_CVRefId",
                        column: x => x.CVRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Deployments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    NextSequence = table.Column<int>(type: "INTEGER", nullable: false),
                    NextSequenceDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deployments_CVRefs_CVRefId",
                        column: x => x.CVRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectionDecisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SelectionStatus = table.Column<string>(type: "TEXT", nullable: true),
                    SelectedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectionDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectionDecisions_CVRefs_CVRefId",
                        column: x => x.CVRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HRSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    IndustryId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HRSkills_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtherSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherSkills_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FeedbackId = table.Column<int>(type: "INTEGER", nullable: false),
                    FeedbackGroup = table.Column<string>(type: "TEXT", nullable: true),
                    FeedbackQNo = table.Column<int>(type: "INTEGER", nullable: false),
                    FeedbackQuestion = table.Column<string>(type: "TEXT", nullable: true),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false),
                    Response = table.Column<string>(type: "TEXT", nullable: true),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    TextForLevel1 = table.Column<string>(type: "TEXT", nullable: true),
                    TextForLevel2 = table.Column<string>(type: "TEXT", nullable: true),
                    TextForLevel3 = table.Column<string>(type: "TEXT", nullable: true),
                    TextForLevel4 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackItems_Feedbacks_FeedbackId",
                        column: x => x.FeedbackId,
                        principalTable: "Feedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FinanceVoucherId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    COAId = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountName = table.Column<string>(type: "TEXT", nullable: true),
                    Dr = table.Column<long>(type: "INTEGER", nullable: false),
                    Cr = table.Column<long>(type: "INTEGER", nullable: false),
                    Narration = table.Column<string>(type: "TEXT", nullable: true),
                    DrEntryApprovedByEmployeeById = table.Column<int>(type: "INTEGER", nullable: false),
                    DrEntryApprovedOn = table.Column<DateTime>(type: "TEXT", maxLength: 10, nullable: false),
                    DrEntryApproved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherEntries_FinanceVouchers_FinanceVoucherId",
                        column: x => x.FinanceVoucherId,
                        principalTable: "FinanceVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerIndustries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: true),
                    IndustryId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomerOfficialId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerIndustries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerIndustries_CustomerOfficials_CustomerOfficialId",
                        column: x => x.CustomerOfficialId,
                        principalTable: "CustomerOfficials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerIndustries_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerIndustries_Industries_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "Industries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContractReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    ReviewedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReviewStatus = table.Column<string>(type: "TEXT", nullable: true),
                    ReleasedForProduction = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractReviews_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DLForwardedToAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerOfficialId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateForwarded = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionName = table.Column<string>(type: "TEXT", nullable: true),
                    SourceFrom = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    MinCVs = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxCVs = table.Column<int>(type: "INTEGER", nullable: false),
                    Ecnr = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompleteBefore = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    FeeFromClientINR = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Checked = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReviewItemStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Professions_ProfessionId",
                        column: x => x.ProfessionId,
                        principalTable: "Professions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserExps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Employer = table.Column<string>(type: "TEXT", nullable: true),
                    Position = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentJob = table.Column<bool>(type: "INTEGER", nullable: true),
                    SalaryCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    MonthlySalaryDrawn = table.Column<int>(type: "INTEGER", nullable: true),
                    WorkedFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WorkedUpto = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserExps_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    MobileNo = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPhones_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    IndustryId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMain = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfessions_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectionDecisionId = table.Column<int>(type: "INTEGER", nullable: false),
                    SelectedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChargesFixed = table.Column<int>(type: "INTEGER", nullable: false),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    SalaryCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    Salary = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractPeriodInMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    WeeklyHours = table.Column<int>(type: "INTEGER", nullable: false),
                    HousingProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    HousingAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    FoodProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    FoodAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    TransportProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    TransportAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    OtherAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    LeavePerYearInDays = table.Column<int>(type: "INTEGER", nullable: false),
                    LeaveAirfareEntitlementAfterMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    OfferAcceptedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OfferAttachmentUrl = table.Column<string>(type: "TEXT", nullable: true),
                    OfferAcceptanceUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employments_SelectionDecisions_SelectionDecisionId",
                        column: x => x.SelectionDecisionId,
                        principalTable: "SelectionDecisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AssessedByEmployeeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessedByEmployeeName = table.Column<string>(type: "TEXT", nullable: true),
                    requireInternalReview = table.Column<bool>(type: "INTEGER", nullable: false),
                    ChecklistHRId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessResult = table.Column<string>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskIdDocControllerAdmin = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateAssessments_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistHRs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HrExecComments = table.Column<string>(type: "TEXT", nullable: true),
                    Charges = table.Column<int>(type: "INTEGER", nullable: false),
                    ChargesAgreed = table.Column<int>(type: "INTEGER", nullable: false),
                    ExceptionApproved = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExceptionApprovedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ExceptionApprovedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChecklistedOk = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistHRs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistHRs_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistHRs_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractReviewItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContractReviewId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Ecnr = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequireAssess = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReviewItemStatus = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractReviewItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractReviewItems_ContractReviews_ContractReviewId",
                        column: x => x.ContractReviewId,
                        principalTable: "ContractReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractReviewItems_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    JobDescInBrief = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    QualificationDesired = table.Column<string>(type: "TEXT", nullable: true),
                    ExpDesiredMin = table.Column<int>(type: "INTEGER", nullable: false),
                    ExpDesiredMax = table.Column<int>(type: "INTEGER", nullable: false),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAge = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobDescriptions_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemAssessmentQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Question = table.Column<string>(type: "TEXT", nullable: true),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemAssessmentQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemAssessmentQs_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Remunerations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkHours = table.Column<int>(type: "INTEGER", nullable: false),
                    SalaryCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    SalaryMin = table.Column<int>(type: "INTEGER", nullable: false),
                    SalaryMax = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractPeriodInMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    HousingProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    HousingAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    HousingNotProvided = table.Column<bool>(type: "INTEGER", nullable: false),
                    FoodProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    FoodAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    FoodNotProvided = table.Column<bool>(type: "INTEGER", nullable: false),
                    TransportProvidedFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    TransportAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    TransportNotProvided = table.Column<bool>(type: "INTEGER", nullable: false),
                    OtherAllowance = table.Column<int>(type: "INTEGER", nullable: false),
                    LeavePerYearInDays = table.Column<int>(type: "INTEGER", nullable: false),
                    LeaveAirfareEntitlementAfterMonths = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remunerations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remunerations_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidatesItemAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CandidateAssessmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    AssessmentGroup = table.Column<string>(type: "TEXT", nullable: true),
                    Question = table.Column<string>(type: "TEXT", nullable: true),
                    IsMandatory = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssessedOnTheParameter = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatesItemAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatesItemAssessments_CandidateAssessments_CandidateAssessmentId",
                        column: x => x.CandidateAssessmentId,
                        principalTable: "CandidateAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistHRItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChecklistHRId = table.Column<int>(type: "INTEGER", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Parameter = table.Column<string>(type: "TEXT", nullable: true),
                    Accepts = table.Column<bool>(type: "INTEGER", nullable: false),
                    Response = table.Column<string>(type: "TEXT", nullable: true),
                    Exceptions = table.Column<string>(type: "TEXT", nullable: true),
                    MandatoryTrue = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistHRItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistHRItems_ChecklistHRs_ChecklistHRId",
                        column: x => x.ChecklistHRId,
                        principalTable: "ChecklistHRs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractReviewItemQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractReviewItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    SrNo = table.Column<int>(type: "INTEGER", nullable: false),
                    ReviewParameter = table.Column<string>(type: "TEXT", nullable: true),
                    Response = table.Column<bool>(type: "INTEGER", nullable: false),
                    ResponseText = table.Column<string>(type: "TEXT", nullable: true),
                    IsResponseBoolean = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMandatoryTrue = table.Column<bool>(type: "INTEGER", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractReviewItemQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractReviewItemQs_ContractReviewItems_ContractReviewItemId",
                        column: x => x.ContractReviewItemId,
                        principalTable: "ContractReviewItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgencySpecialties_CustomerId",
                table: "AgencySpecialties",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQStdds_Question",
                table: "AssessmentQStdds",
                column: "Question",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQStdds_QuestionNo",
                table: "AssessmentQStdds",
                column: "QuestionNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAssessments_OrderItemId",
                table: "CandidateAssessments",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_CVRefId",
                table: "Candidates",
                column: "CVRefId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatesItemAssessments_CandidateAssessmentId",
                table: "CandidatesItemAssessments",
                column: "CandidateAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRItems_ChecklistHRId",
                table: "ChecklistHRItems",
                column: "ChecklistHRId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRs_CandidateId",
                table: "ChecklistHRs",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistHRs_OrderItemId",
                table: "ChecklistHRs",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_COAs_AccountName",
                table: "COAs",
                column: "AccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviewItemQs_ContractReviewItemId",
                table: "ContractReviewItemQs",
                column: "ContractReviewItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviewItems_ContractReviewId",
                table: "ContractReviewItems",
                column: "ContractReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviewItems_OrderItemId",
                table: "ContractReviewItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractReviews_OrderId",
                table: "ContractReviews",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIndustries_CustomerId",
                table: "CustomerIndustries",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIndustries_CustomerOfficialId",
                table: "CustomerIndustries",
                column: "CustomerOfficialId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIndustries_IndustryId",
                table: "CustomerIndustries",
                column: "IndustryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOfficials_CustomerId_OfficialName",
                table: "CustomerOfficials",
                columns: new[] { "CustomerId", "OfficialName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerName_City",
                table: "Customers",
                columns: new[] { "CustomerName", "City" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CVRefs_OrderItemId_CandidateId",
                table: "CVRefs",
                columns: new[] { "OrderItemId", "CandidateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deployments_CVRefId",
                table: "Deployments",
                column: "CVRefId");

            migrationBuilder.CreateIndex(
                name: "IX_DLForwardedToAgents_CustomerOfficialId_DateForwarded_OrderItemId",
                table: "DLForwardedToAgents",
                columns: new[] { "CustomerOfficialId", "DateForwarded", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DLForwardedToAgents_OrderId",
                table: "DLForwardedToAgents",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Employments_SelectionDecisionId",
                table: "Employments",
                column: "SelectionDecisionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackItems_FeedbackId",
                table: "FeedbackItems",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_HRSkills_EmployeeId",
                table: "HRSkills",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDescriptions_OrderItemId",
                table: "JobDescriptions",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemAssessmentQs_OrderItemId",
                table: "OrderItemAssessmentQs",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProfessionId",
                table: "OrderItems",
                column: "ProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherSkills_EmployeeId",
                table: "OtherSkills",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Professions_ProfessionName",
                table: "Professions",
                column: "ProfessionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Remunerations_OrderItemId",
                table: "Remunerations",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelectionDecisions_CVRefId",
                table: "SelectionDecisions",
                column: "CVRefId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExps_CandidateId",
                table: "UserExps",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPhones_CandidateId",
                table: "UserPhones",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfessions_CandidateId",
                table: "UserProfessions",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherEntries_FinanceVoucherId",
                table: "VoucherEntries",
                column: "FinanceVoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLike_AspNetUsers_SourceUserId",
                table: "UserLike",
                column: "SourceUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLike_AspNetUsers_TargetUserId",
                table: "UserLike",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLike_AspNetUsers_SourceUserId",
                table: "UserLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLike_AspNetUsers_TargetUserId",
                table: "UserLike");

            migrationBuilder.DropTable(
                name: "AgencySpecialties");

            migrationBuilder.DropTable(
                name: "AssessmentQStdds");

            migrationBuilder.DropTable(
                name: "CandidatesItemAssessments");

            migrationBuilder.DropTable(
                name: "CategoryAssessmentQBanks");

            migrationBuilder.DropTable(
                name: "ChecklistHRDatas");

            migrationBuilder.DropTable(
                name: "ChecklistHRItems");

            migrationBuilder.DropTable(
                name: "COAs");

            migrationBuilder.DropTable(
                name: "ContractReviewItemQs");

            migrationBuilder.DropTable(
                name: "CustomerIndustries");

            migrationBuilder.DropTable(
                name: "Deployments");

            migrationBuilder.DropTable(
                name: "DeployStatuses");

            migrationBuilder.DropTable(
                name: "DLForwardedToAgents");

            migrationBuilder.DropTable(
                name: "Employments");

            migrationBuilder.DropTable(
                name: "FeedbackItems");

            migrationBuilder.DropTable(
                name: "HRSkills");

            migrationBuilder.DropTable(
                name: "JobDescriptions");

            migrationBuilder.DropTable(
                name: "OrderItemAssessmentQs");

            migrationBuilder.DropTable(
                name: "OtherSkills");

            migrationBuilder.DropTable(
                name: "Remunerations");

            migrationBuilder.DropTable(
                name: "ReviewItemDatas");

            migrationBuilder.DropTable(
                name: "SkillDatas");

            migrationBuilder.DropTable(
                name: "UserExps");

            migrationBuilder.DropTable(
                name: "UserPhones");

            migrationBuilder.DropTable(
                name: "UserProfessions");

            migrationBuilder.DropTable(
                name: "UserQualifications");

            migrationBuilder.DropTable(
                name: "VoucherEntries");

            migrationBuilder.DropTable(
                name: "CandidateAssessments");

            migrationBuilder.DropTable(
                name: "ChecklistHRs");

            migrationBuilder.DropTable(
                name: "ContractReviewItems");

            migrationBuilder.DropTable(
                name: "CustomerOfficials");

            migrationBuilder.DropTable(
                name: "Industries");

            migrationBuilder.DropTable(
                name: "SelectionDecisions");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "FinanceVouchers");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "ContractReviews");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "CVRefs");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Professions");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLike",
                table: "UserLike");

            migrationBuilder.RenameTable(
                name: "UserLike",
                newName: "Likes");

            migrationBuilder.RenameIndex(
                name: "IX_UserLike_TargetUserId",
                table: "Likes",
                newName: "IX_Likes_TargetUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                columns: new[] { "SourceUserId", "TargetUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_SourceUserId",
                table: "Likes",
                column: "SourceUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_TargetUserId",
                table: "Likes",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
