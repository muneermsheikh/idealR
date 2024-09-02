using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Data.Migrations
{
    /// <inheritdoc />
    public partial class HRSkills_LevelName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillLevel",
                table: "HRSkills");

            migrationBuilder.AddColumn<string>(
                name: "SkillLevelName",
                table: "HRSkills",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillLevelName",
                table: "HRSkills");

            migrationBuilder.AddColumn<int>(
                name: "SkillLevel",
                table: "HRSkills",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
