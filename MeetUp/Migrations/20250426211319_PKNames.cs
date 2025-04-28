using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetUp.Migrations
{
    /// <inheritdoc />
    public partial class PKNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClassRef",
                table: "Gym_Users",
                newName: "Gym_UsersClassRef");

            migrationBuilder.RenameColumn(
                name: "ClassRef",
                table: "Gym_Sets",
                newName: "Gym_SetsClassRef");

            migrationBuilder.RenameColumn(
                name: "ClassRef",
                table: "Gym_Sessions",
                newName: "Gym_SessionsClassRef");

            migrationBuilder.RenameColumn(
                name: "ClassRef",
                table: "Gym_Repetitions",
                newName: "Gym_RepetitionsClassRef");

            migrationBuilder.RenameColumn(
                name: "ClassRef",
                table: "Gym_Exercises",
                newName: "Gym_ExercisesClassRef");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gym_UsersClassRef",
                table: "Gym_Users",
                newName: "ClassRef");

            migrationBuilder.RenameColumn(
                name: "Gym_SetsClassRef",
                table: "Gym_Sets",
                newName: "ClassRef");

            migrationBuilder.RenameColumn(
                name: "Gym_SessionsClassRef",
                table: "Gym_Sessions",
                newName: "ClassRef");

            migrationBuilder.RenameColumn(
                name: "Gym_RepetitionsClassRef",
                table: "Gym_Repetitions",
                newName: "ClassRef");

            migrationBuilder.RenameColumn(
                name: "Gym_ExercisesClassRef",
                table: "Gym_Exercises",
                newName: "ClassRef");
        }
    }
}
