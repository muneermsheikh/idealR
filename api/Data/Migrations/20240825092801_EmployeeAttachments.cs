using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeeAddress",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeePhone",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeePhone2",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeeQualifications",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Qualifications",
                table: "Employees",
                newName: "Qualification");

            migrationBuilder.RenameColumn(
                name: "OfficialPhoneNo",
                table: "Employees",
                newName: "PhoneNo");

            migrationBuilder.RenameColumn(
                name: "OfficialMobileNo",
                table: "Employees",
                newName: "Phone2");

            migrationBuilder.RenameColumn(
                name: "OfficialEmail",
                table: "Employees",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Nationality",
                table: "Employees",
                newName: "DisplayName");

            migrationBuilder.CreateTable(
                name: "EmployeeAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAttachments_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAttachments_EmployeeId",
                table: "EmployeeAttachments",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeAttachments");

            migrationBuilder.RenameColumn(
                name: "Qualification",
                table: "Employees",
                newName: "Qualifications");

            migrationBuilder.RenameColumn(
                name: "PhoneNo",
                table: "Employees",
                newName: "OfficialPhoneNo");

            migrationBuilder.RenameColumn(
                name: "Phone2",
                table: "Employees",
                newName: "OfficialMobileNo");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Employees",
                newName: "OfficialEmail");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Employees",
                newName: "Nationality");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeAddress",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeePhone",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeePhone2",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeQualifications",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
