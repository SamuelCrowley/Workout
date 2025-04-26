using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetUp.Migrations
{
    /// <inheritdoc />
    public partial class MissingTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentRefType",
                table: "Gym_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParentRefType",
                table: "Gym_Sets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParentRefType",
                table: "Gym_Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParentRefType",
                table: "Gym_Repetitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParentRefType",
                table: "Gym_Exercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentRefType",
                table: "Gym_Users");

            migrationBuilder.DropColumn(
                name: "ParentRefType",
                table: "Gym_Sets");

            migrationBuilder.DropColumn(
                name: "ParentRefType",
                table: "Gym_Sessions");

            migrationBuilder.DropColumn(
                name: "ParentRefType",
                table: "Gym_Repetitions");

            migrationBuilder.DropColumn(
                name: "ParentRefType",
                table: "Gym_Exercises");
        }
    }
}
