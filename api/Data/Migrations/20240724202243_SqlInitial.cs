using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SqlInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AckanowledgeToClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    DateAcknowledged = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipientUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecipientEmailId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderEmailId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AckanowledgeToClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KnownAs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActive = table.Column<DateTime>(type: "datetime2", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentQBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentQStdds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionNo = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentQStdds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CallRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PersonName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcludedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidateFlights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepId = table.Column<int>(type: "int", nullable: false),
                    CvRefId = table.Column<int>(type: "int", nullable: false),
                    DepItemId = table.Column<int>(type: "int", nullable: false),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfFlight = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlightNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirportOfBoarding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirportOfDestination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ETD_Boarding = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ETA_Destination = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AirportVia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FightNoVia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ETA_Via = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ETD_Via = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateFlights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAssessmentQBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentQBankId = table.Column<int>(type: "int", nullable: false),
                    AssessmentParameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QNo = table.Column<int>(type: "int", nullable: false),
                    IsStandardQ = table.Column<bool>(type: "bit", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAssessmentQBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistHRDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SrNo = table.Column<int>(type: "int", nullable: false),
                    Parameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistHRDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COAs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Divn = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpBalance = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COAs", x => x.Id);
                });

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
                name: "ContractReviewItemStddQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SrNo = table.Column<int>(type: "int", nullable: false),
                    ReviewParameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Button2Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextInput = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatoryTrue = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractReviewItemStddQs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedbackNo = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialAppUserId = table.Column<int>(type: "int", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateIssued = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HowReceived = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GradeAssessedByClient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KnownAs = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Add = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Add2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Introduction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBlackListed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CVRefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateAssessmentId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    ReferredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HRExecUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefStatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SelectionStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectionStatusDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVRefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deployments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CVRefId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    NextSequence = table.Column<int>(type: "int", nullable: false),
                    NextSequenceDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeployStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isOptional = table.Column<bool>(type: "bit", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    NextSequence = table.Column<int>(type: "int", nullable: false),
                    WorkingDaysReqdForNextStage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeployStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FamilyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KnownAs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qualifications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlaceOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AadharNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficialMobileNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeePhone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeQualifications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CvRefId = table.Column<int>(type: "int", nullable: false),
                    SelectionDecisionId = table.Column<int>(type: "int", nullable: false),
                    SelectedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChargesFixed = table.Column<int>(type: "int", nullable: false),
                    Charges = table.Column<int>(type: "int", nullable: false),
                    SalaryCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salary = table.Column<int>(type: "int", nullable: false),
                    ContractPeriodInMonths = table.Column<int>(type: "int", nullable: false),
                    WeeklyHours = table.Column<int>(type: "int", nullable: false),
                    HousingProvidedFree = table.Column<bool>(type: "bit", nullable: false),
                    HousingNotProvided = table.Column<bool>(type: "bit", nullable: false),
                    HousingAllowance = table.Column<int>(type: "int", nullable: false),
                    FoodProvidedFree = table.Column<bool>(type: "bit", nullable: false),
                    FoodAllowance = table.Column<int>(type: "int", nullable: false),
                    FoodNotProvided = table.Column<bool>(type: "bit", nullable: false),
                    TransportProvidedFree = table.Column<bool>(type: "bit", nullable: false),
                    TransportAllowance = table.Column<int>(type: "int", nullable: false),
                    TransportNotProvided = table.Column<bool>(type: "bit", nullable: false),
                    OtherAllowance = table.Column<int>(type: "int", nullable: false),
                    LeavePerYearInDays = table.Column<int>(type: "int", nullable: false),
                    LeaveAirfareEntitlementAfterMonths = table.Column<int>(type: "int", nullable: false),
                    OfferAccepted = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfferAcceptanceConcludedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OfferConclusionRegisteredByUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfferAttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfferAcceptanceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedbackGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionNo = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prompt1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prompt2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prompt3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prompt4 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackQs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinanceVouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Divn = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    CoaId = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoucherNo = table.Column<int>(type: "int", nullable: false),
                    VoucherDated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceVouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlightDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AirportOfBoarding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirportOfDestination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirportVia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FightNoVia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ETD_Boarding = table.Column<TimeSpan>(type: "time", nullable: false),
                    ETA_Destination = table.Column<TimeSpan>(type: "time", nullable: false),
                    ETA_Via = table.Column<TimeSpan>(type: "time", nullable: false),
                    ETD_Via = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Helps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Topic = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Helps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Industries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndustryName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Industries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageComposeSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SrNo = table.Column<int>(type: "int", nullable: false),
                    LineText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageComposeSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DesignedByUsername = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderForwardCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Charges = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderForwardCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderForwardToHRs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    RecipientUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOnlyForwarded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderForwardToHRs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orderItemAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    RequireCandidateAssessment = table.Column<bool>(type: "bit", nullable: false),
                    AssessmentRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDesigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DesignedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderItemAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfessionName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProspectiveCandidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryRef = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonId = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResumeTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CandidateName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Age = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    AlternateNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternateEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Education = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ctc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProspectiveCandidates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QualificationName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CVRefId = table.Column<int>(type: "int", nullable: true),
                    TaskType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CandidateAssessmentId = table.Column<int>(type: "int", nullable: true),
                    TaskDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskOwnerUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedToUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: true),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ApplicationNo = table.Column<int>(type: "int", nullable: true),
                    ResumeId = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompleteBy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HistoryItemId = table.Column<int>(type: "int", nullable: true),
                    PostTaskAction = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CVRefId = table.Column<int>(type: "int", nullable: false),
                    Divn = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    COAId = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoucherNo = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    VoucherDated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CvRefId = table.Column<int>(type: "int", nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAppUserId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: true),
                    SenderUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecipientId = table.Column<int>(type: "int", nullable: true),
                    RecipientAppUserId = table.Column<int>(type: "int", nullable: false),
                    RecipientUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecipientEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CCEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BCCEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageComposedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageSentOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SenderDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RecipientDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLike",
                columns: table => new
                {
                    SourceUserId = table.Column<int>(type: "int", nullable: false),
                    TargetUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLike", x => new { x.SourceUserId, x.TargetUserId });
                    table.ForeignKey(
                        name: "FK_UserLike_AspNetUsers_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLike_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssessmentStddQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentQBankId = table.Column<int>(type: "int", nullable: false),
                    AssessmentParameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QNo = table.Column<int>(type: "int", nullable: false),
                    IsStandardQ = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxPoints = table.Column<int>(type: "int", nullable: false)
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
                name: "CallRecordItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CallRecordId = table.Column<int>(type: "int", nullable: false),
                    IncomingOutgoing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfContact = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GistOfDiscussions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextActionOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdvisoryBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallRecordItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CallRecordItems_CallRecords_CallRecordId",
                        column: x => x.CallRecordId,
                        principalTable: "CallRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerFeedbackId = table.Column<int>(type: "int", nullable: false),
                    FeedbackGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionNo = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Prompt1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prompt2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prompt3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Prompt4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Response = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackItems_CustomerFeedbacks_CustomerFeedbackId",
                        column: x => x.CustomerFeedbackId,
                        principalTable: "CustomerFeedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReviewItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerReviewId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerReviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedByUsername = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "AgencySpecialties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    IndustryId = table.Column<int>(type: "int", nullable: true),
                    SpecialtyName = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    OfficialName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KnownAs = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Divn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriorityHR = table.Column<int>(type: "int", nullable: false),
                    PriorityAdmin = table.Column<int>(type: "int", nullable: false),
                    PriorityAccount = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderRefDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectManagerId = table.Column<int>(type: "int", nullable: false),
                    SalesmanId = table.Column<int>(type: "int", nullable: true),
                    CompleteBy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityOfWorking = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractReviewId = table.Column<int>(type: "int", nullable: false),
                    ContractReviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ForwardedToHRDeptOn = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FamilyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KnownAs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    ReferredByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PpNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AadharNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ecnr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActive = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppUserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qualifications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationDesired = table.Column<bool>(type: "bit", nullable: false),
                    CVRefId = table.Column<int>(type: "int", nullable: true)
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
                name: "Deps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CvRefId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityOfWorking = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentStatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ecnr = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deps_CVRefs_CvRefId",
                        column: x => x.CvRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Processes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CVRefId = table.Column<int>(type: "int", nullable: false),
                    SelectedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Processes_CVRefs_CVRefId",
                        column: x => x.CVRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectionDecisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CvRefId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ApplicationNo = table.Column<int>(type: "int", nullable: false),
                    CandidateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectedAs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityOfWorking = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectionStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Charges = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectionDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectionDecisions_CVRefs_CvRefId",
                        column: x => x.CvRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HRSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndustryId = table.Column<int>(type: "int", nullable: false),
                    SkillLevel = table.Column<int>(type: "int", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SkillDataId = table.Column<int>(type: "int", nullable: false),
                    SkillLevel = table.Column<int>(type: "int", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false)
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
                name: "VoucherEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinanceVoucherId = table.Column<int>(type: "int", nullable: false),
                    TransDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CoaId = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dr = table.Column<long>(type: "bigint", nullable: false),
                    Cr = table.Column<long>(type: "bigint", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrEntryApprovedByUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrEntryApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DrEntryApproved = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "HelpItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HelpId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    HelpText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HelpItems_Helps_HelpId",
                        column: x => x.HelpId,
                        principalTable: "Helps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerIndustries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    IndustryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerIndustries", x => x.Id);
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
                name: "OrderAssessmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderAssessmentId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    RequireCandidateAssessment = table.Column<bool>(type: "bit", nullable: false),
                    AssessmentRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDesigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DesignedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAssessmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAssessmentItems_OrderAssessments_orderAssessmentId",
                        column: x => x.orderAssessmentId,
                        principalTable: "OrderAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderForwardCategoryOfficials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderForwardCategoryId = table.Column<int>(type: "int", nullable: false),
                    CustomerOfficialId = table.Column<int>(type: "int", nullable: false),
                    OfficialName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AgentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateForwarded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmailIdForwardedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNoForwardedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhatsAppNoForwardedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppTaskId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextFollowupOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextFollowupByName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskItems_Tasks_AppTaskId",
                        column: x => x.AppTaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinanceVoucherId = table.Column<int>(type: "int", nullable: false),
                    AttachmentSizeInBytes = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedByUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoucherId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherAttachments_FinanceVouchers_FinanceVoucherId",
                        column: x => x.FinanceVoucherId,
                        principalTable: "FinanceVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherAttachments_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VoucherItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherId = table.Column<int>(type: "int", nullable: false),
                    TransDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    COAId = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dr = table.Column<long>(type: "bigint", nullable: false),
                    Cr = table.Column<long>(type: "bigint", nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrEntryApprovedByAppUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrEntryApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DrEntryApproved = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherItem_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HrExecAssignedToUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicalProcessInchargeUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisaProcessInchargeUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmigProcessInchargeUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TravelProcessInchargeUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleasedForProduction = table.Column<bool>(type: "bit", nullable: false)
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
                name: "OrderForwardToAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    customerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectManagerId = table.Column<int>(type: "int", nullable: false)
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
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    SrNo = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    SourceFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    MinCVs = table.Column<int>(type: "int", nullable: false),
                    MaxCVs = table.Column<int>(type: "int", nullable: false),
                    Ecnr = table.Column<bool>(type: "bit", nullable: false),
                    CompleteBefore = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Checked = table.Column<bool>(type: "bit", nullable: false),
                    ReviewItemStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<int>(type: "int", nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Length = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedbyUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "UserExps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    SrNo = table.Column<int>(type: "int", nullable: false),
                    Employer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentJob = table.Column<bool>(type: "bit", nullable: true),
                    SalaryCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthlySalaryDrawn = table.Column<int>(type: "int", nullable: true),
                    WorkedFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkedUpto = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    ProfessionId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndustryId = table.Column<int>(type: "int", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false)
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
                name: "UserQualifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    QualificationId = table.Column<int>(type: "int", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserQualifications_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    NextSequence = table.Column<int>(type: "int", nullable: false),
                    NextSequenceDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepItems_Deps_DepId",
                        column: x => x.DepId,
                        principalTable: "Deps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    NextSequence = table.Column<int>(type: "int", nullable: false),
                    NextSequenceDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessItems_Processes_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HelpSubItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    helpId = table.Column<int>(type: "int", nullable: false),
                    sequence = table.Column<int>(type: "int", nullable: false),
                    SubText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HelpItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpSubItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HelpSubItem_HelpItems_HelpItemId",
                        column: x => x.HelpItemId,
                        principalTable: "HelpItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderAssessmentItemQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderAssessmentItemId = table.Column<int>(type: "int", nullable: false),
                    QuestionNo = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAssessmentItemQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAssessmentItemQs_OrderAssessmentItems_OrderAssessmentItemId",
                        column: x => x.OrderAssessmentItemId,
                        principalTable: "OrderAssessmentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryRefAndName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    AssessedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssessedByEmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequireInternalReview = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChecklistHRId = table.Column<int>(type: "int", nullable: false),
                    AssessResult = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CVRefId = table.Column<int>(type: "int", nullable: false),
                    TaskIdDocControllerAdmin = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HrExecUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Charges = table.Column<int>(type: "int", nullable: false),
                    ChargesAgreed = table.Column<int>(type: "int", nullable: false),
                    ExceptionApproved = table.Column<bool>(type: "bit", nullable: false),
                    ExceptionApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExceptionApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChecklistedOk = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractReviewId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProfessionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Ecnr = table.Column<bool>(type: "bit", nullable: false),
                    SourceFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequireAssess = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Charges = table.Column<int>(type: "int", nullable: false),
                    HrExecUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewItemStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "JobDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    JobDescInBrief = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    QualificationDesired = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpDesiredMin = table.Column<int>(type: "int", nullable: false),
                    ExpDesiredMax = table.Column<int>(type: "int", nullable: false),
                    MinAge = table.Column<int>(type: "int", nullable: false),
                    MaxAge = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemAssessmentId = table.Column<int>(type: "int", nullable: false),
                    QuestionNo = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Remunerations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    WorkHours = table.Column<int>(type: "int", nullable: false),
                    SalaryCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SalaryMin = table.Column<int>(type: "int", nullable: false),
                    SalaryMax = table.Column<int>(type: "int", nullable: false),
                    ContractPeriodInMonths = table.Column<int>(type: "int", nullable: false),
                    HousingProvidedFree = table.Column<bool>(type: "bit", nullable: false),
                    HousingAllowance = table.Column<int>(type: "int", nullable: false),
                    HousingNotProvided = table.Column<bool>(type: "bit", nullable: false),
                    FoodProvidedFree = table.Column<bool>(type: "bit", nullable: false),
                    FoodAllowance = table.Column<int>(type: "int", nullable: false),
                    FoodNotProvided = table.Column<bool>(type: "bit", nullable: false),
                    TransportProvidedFree = table.Column<bool>(type: "bit", nullable: false),
                    TransportAllowance = table.Column<int>(type: "int", nullable: false),
                    TransportNotProvided = table.Column<bool>(type: "bit", nullable: false),
                    OtherAllowance = table.Column<int>(type: "int", nullable: false),
                    LeavePerYearInDays = table.Column<int>(type: "int", nullable: false),
                    LeaveAirfareEntitlementAfterMonths = table.Column<int>(type: "int", nullable: false)
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
                name: "CandidatesAssessmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateAssessmentId = table.Column<int>(type: "int", nullable: false),
                    QuestionNo = table.Column<int>(type: "int", nullable: false),
                    AssessmentGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    AssessedOnTheParameter = table.Column<bool>(type: "bit", nullable: false),
                    MaxPoints = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatesAssessmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatesAssessmentItems_CandidateAssessments_CandidateAssessmentId",
                        column: x => x.CandidateAssessmentId,
                        principalTable: "CandidateAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistHRItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecklistHRId = table.Column<int>(type: "int", nullable: false),
                    SrNo = table.Column<int>(type: "int", nullable: false),
                    Parameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accepts = table.Column<bool>(type: "bit", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exceptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MandatoryTrue = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    ContractReviewItemId = table.Column<int>(type: "int", nullable: false),
                    SrNo = table.Column<int>(type: "int", nullable: false),
                    ReviewParameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Response = table.Column<bool>(type: "bit", nullable: false),
                    ResponseText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsResponseBoolean = table.Column<bool>(type: "bit", nullable: false),
                    IsMandatoryTrue = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_AckanowledgeToClients_OrderId",
                table: "AckanowledgeToClients",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgencySpecialties_CustomerId",
                table: "AgencySpecialties",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQBanks_ProfessionId",
                table: "AssessmentQBanks",
                column: "ProfessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQStdds_Question",
                table: "AssessmentQStdds",
                column: "Question",
                unique: true,
                filter: "[Question] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentQStdds_QuestionNo",
                table: "AssessmentQStdds",
                column: "QuestionNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentStddQs_AssessmentQBankId",
                table: "AssessmentStddQs",
                column: "AssessmentQBankId");

            migrationBuilder.CreateIndex(
                name: "IX_CallRecordItems_CallRecordId",
                table: "CallRecordItems",
                column: "CallRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_CallRecords_PersonId",
                table: "CallRecords",
                column: "PersonId",
                unique: true,
                filter: "[PersonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAssessments_CandidateId_OrderItemId",
                table: "CandidateAssessments",
                columns: new[] { "CandidateId", "OrderItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateAssessments_OrderItemId",
                table: "CandidateAssessments",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateFlights_CvRefId",
                table: "CandidateFlights",
                column: "CvRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_CVRefId",
                table: "Candidates",
                column: "CVRefId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatesAssessmentItems_CandidateAssessmentId",
                table: "CandidatesAssessmentItems",
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
                name: "IX_ChecklistHRs_OrderItemId_CandidateId",
                table: "ChecklistHRs",
                columns: new[] { "OrderItemId", "CandidateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_COAs_AccountName",
                table: "COAs",
                column: "AccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contactResults_Status",
                table: "contactResults",
                column: "Status",
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
                column: "OrderItemId",
                unique: true);

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
                name: "IX_CustomerIndustries_IndustryId",
                table: "CustomerIndustries",
                column: "IndustryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOfficials_CustomerId_OfficialName",
                table: "CustomerOfficials",
                columns: new[] { "CustomerId", "OfficialName" },
                unique: true);

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
                name: "IX_DepItems_DepId_Sequence",
                table: "DepItems",
                columns: new[] { "DepId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deps_CvRefId",
                table: "Deps",
                column: "CvRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_CvRefId",
                table: "Employments",
                column: "CvRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employments_SelectionDecisionId",
                table: "Employments",
                column: "SelectionDecisionId",
                filter: "SelectionDecisionId is NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackItems_CustomerFeedbackId",
                table: "FeedbackItems",
                column: "CustomerFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightDetails_FlightNo",
                table: "FlightDetails",
                column: "FlightNo",
                unique: true,
                filter: "[FlightNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HelpItems_HelpId_Sequence",
                table: "HelpItems",
                columns: new[] { "HelpId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Helps_Topic",
                table: "Helps",
                column: "Topic",
                unique: true,
                filter: "[Topic] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HelpSubItem_HelpItemId",
                table: "HelpSubItem",
                column: "HelpItemId");

            migrationBuilder.CreateIndex(
                name: "IX_HRSkills_EmployeeId",
                table: "HRSkills",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Industries_IndustryName",
                table: "Industries",
                column: "IndustryName",
                unique: true,
                filter: "[IndustryName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JobDescriptions_OrderItemId",
                table: "JobDescriptions",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessmentItemQs_OrderAssessmentItemId_Question",
                table: "OrderAssessmentItemQs",
                columns: new[] { "OrderAssessmentItemId", "Question" },
                unique: true,
                filter: "[Question] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessmentItems_orderAssessmentId",
                table: "OrderAssessmentItems",
                column: "orderAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessmentItems_OrderItemId",
                table: "OrderAssessmentItems",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessments_OrderId",
                table: "OrderAssessments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardCategories_OrderItemId",
                table: "OrderForwardCategories",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderForwardCategoryOfficials_OrderForwardCategoryId_DateForwarded_CustomerOfficialId",
                table: "OrderForwardCategoryOfficials",
                columns: new[] { "OrderForwardCategoryId", "DateForwarded", "CustomerOfficialId" },
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
                name: "IX_OrderItemAssessmentQs_OrderItemAssessmentId",
                table: "OrderItemAssessmentQs",
                column: "OrderItemAssessmentId");

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
                name: "IX_Photos_AppUserId",
                table: "Photos",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Processes_CVRefId",
                table: "Processes",
                column: "CVRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessItems_ProcessId",
                table: "ProcessItems",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Professions_ProfessionName",
                table: "Professions",
                column: "ProfessionName",
                unique: true,
                filter: "[ProfessionName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveCandidates_PersonId",
                table: "ProspectiveCandidates",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_QualificationName",
                table: "Qualifications",
                column: "QualificationName",
                unique: true,
                filter: "[QualificationName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Remunerations_OrderItemId",
                table: "Remunerations",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SelectionDecisions_CvRefId",
                table: "SelectionDecisions",
                column: "CvRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_AppTaskId",
                table: "TaskItems",
                column: "AppTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskType",
                table: "Tasks",
                column: "TaskType");

            migrationBuilder.CreateIndex(
                name: "IX_UserAttachments_CandidateId",
                table: "UserAttachments",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserExps_CandidateId",
                table: "UserExps",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLike_TargetUserId",
                table: "UserLike",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPhones_CandidateId",
                table: "UserPhones",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfessions_CandidateId",
                table: "UserProfessions",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserQualifications_CandidateId",
                table: "UserQualifications",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_FileName_FinanceVoucherId",
                table: "VoucherAttachments",
                columns: new[] { "FileName", "FinanceVoucherId" },
                unique: true,
                filter: "[FileName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_FinanceVoucherId",
                table: "VoucherAttachments",
                column: "FinanceVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAttachments_VoucherId",
                table: "VoucherAttachments",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherEntries_FinanceVoucherId",
                table: "VoucherEntries",
                column: "FinanceVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherItem_VoucherId",
                table: "VoucherItem",
                column: "VoucherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AckanowledgeToClients");

            migrationBuilder.DropTable(
                name: "AgencySpecialties");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AssessmentQStdds");

            migrationBuilder.DropTable(
                name: "AssessmentStddQs");

            migrationBuilder.DropTable(
                name: "CallRecordItems");

            migrationBuilder.DropTable(
                name: "CandidateFlights");

            migrationBuilder.DropTable(
                name: "CandidatesAssessmentItems");

            migrationBuilder.DropTable(
                name: "CategoryAssessmentQBanks");

            migrationBuilder.DropTable(
                name: "ChecklistHRDatas");

            migrationBuilder.DropTable(
                name: "ChecklistHRItems");

            migrationBuilder.DropTable(
                name: "COAs");

            migrationBuilder.DropTable(
                name: "contactResults");

            migrationBuilder.DropTable(
                name: "ContractReviewItemQs");

            migrationBuilder.DropTable(
                name: "ContractReviewItemStddQs");

            migrationBuilder.DropTable(
                name: "CustomerIndustries");

            migrationBuilder.DropTable(
                name: "CustomerOfficials");

            migrationBuilder.DropTable(
                name: "CustomerReviewItems");

            migrationBuilder.DropTable(
                name: "DepItems");

            migrationBuilder.DropTable(
                name: "Deployments");

            migrationBuilder.DropTable(
                name: "DeployStatuses");

            migrationBuilder.DropTable(
                name: "Employments");

            migrationBuilder.DropTable(
                name: "FeedbackItems");

            migrationBuilder.DropTable(
                name: "FeedbackQs");

            migrationBuilder.DropTable(
                name: "FlightDetails");

            migrationBuilder.DropTable(
                name: "HelpSubItem");

            migrationBuilder.DropTable(
                name: "HRSkills");

            migrationBuilder.DropTable(
                name: "JobDescriptions");

            migrationBuilder.DropTable(
                name: "MessageComposeSources");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "OrderAssessmentItemQs");

            migrationBuilder.DropTable(
                name: "OrderForwardCategoryOfficials");

            migrationBuilder.DropTable(
                name: "OrderForwardToAgents");

            migrationBuilder.DropTable(
                name: "OrderForwardToHRs");

            migrationBuilder.DropTable(
                name: "OrderItemAssessmentQs");

            migrationBuilder.DropTable(
                name: "OtherSkills");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "ProcessItems");

            migrationBuilder.DropTable(
                name: "ProspectiveCandidates");

            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropTable(
                name: "Remunerations");

            migrationBuilder.DropTable(
                name: "SelectionDecisions");

            migrationBuilder.DropTable(
                name: "SkillDatas");

            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "UserAttachments");

            migrationBuilder.DropTable(
                name: "UserExps");

            migrationBuilder.DropTable(
                name: "UserLike");

            migrationBuilder.DropTable(
                name: "UserPhones");

            migrationBuilder.DropTable(
                name: "UserProfessions");

            migrationBuilder.DropTable(
                name: "UserQualifications");

            migrationBuilder.DropTable(
                name: "VoucherAttachments");

            migrationBuilder.DropTable(
                name: "VoucherEntries");

            migrationBuilder.DropTable(
                name: "VoucherItem");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AssessmentQBanks");

            migrationBuilder.DropTable(
                name: "CallRecords");

            migrationBuilder.DropTable(
                name: "CandidateAssessments");

            migrationBuilder.DropTable(
                name: "ChecklistHRs");

            migrationBuilder.DropTable(
                name: "ContractReviewItems");

            migrationBuilder.DropTable(
                name: "Industries");

            migrationBuilder.DropTable(
                name: "CustomerReviews");

            migrationBuilder.DropTable(
                name: "Deps");

            migrationBuilder.DropTable(
                name: "CustomerFeedbacks");

            migrationBuilder.DropTable(
                name: "HelpItems");

            migrationBuilder.DropTable(
                name: "OrderAssessmentItems");

            migrationBuilder.DropTable(
                name: "OrderForwardCategories");

            migrationBuilder.DropTable(
                name: "orderItemAssessments");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Processes");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FinanceVouchers");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "ContractReviews");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Helps");

            migrationBuilder.DropTable(
                name: "OrderAssessments");

            migrationBuilder.DropTable(
                name: "CVRefs");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Professions");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
