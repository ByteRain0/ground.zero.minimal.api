using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HabitTrackerAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class DayInformation_RemoveColumn_Checked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Checked",
                table: "DaysInformation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Checked",
                table: "DaysInformation",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
