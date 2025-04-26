using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetUp.Migrations
{
    /// <inheritdoc />
    public partial class ParentRefTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentRefType",
                table: "Gym_Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentRefType",
                table: "Gym_Sets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentRefType",
                table: "Gym_Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentRefType",
                table: "Gym_Repetitions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentRefType",
                table: "Gym_Exercises",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
