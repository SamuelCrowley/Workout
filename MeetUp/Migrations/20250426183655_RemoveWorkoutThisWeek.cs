using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetUp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWorkoutThisWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gym_Exercises_Gym_Sessions_ParentRef",
                table: "Gym_Exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_Gym_Repetitions_Gym_Sets_ParentRef",
                table: "Gym_Repetitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Gym_Sessions_Gym_Users_ParentRef",
                table: "Gym_Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Gym_Sets_Gym_Exercises_ParentRef",
                table: "Gym_Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Gym_Users_AspNetUsers_ParentRef",
                table: "Gym_Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gym_Users",
                table: "Gym_Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gym_Sets",
                table: "Gym_Sets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gym_Sessions",
                table: "Gym_Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gym_Repetitions",
                table: "Gym_Repetitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gym_Exercises",
                table: "Gym_Exercises");

            migrationBuilder.DropColumn(
                name: "WorkoutsThisWeek",
                table: "Gym_Users");

            migrationBuilder.RenameTable(
                name: "Gym_Users",
                newName: "GymUsers");

            migrationBuilder.RenameTable(
                name: "Gym_Sets",
                newName: "GymSetEO");

            migrationBuilder.RenameTable(
                name: "Gym_Sessions",
                newName: "GymSession");

            migrationBuilder.RenameTable(
                name: "Gym_Repetitions",
                newName: "GymRepetition");

            migrationBuilder.RenameTable(
                name: "Gym_Exercises",
                newName: "GymExerciseEO");

            migrationBuilder.RenameIndex(
                name: "IX_Gym_Users_ParentRef",
                table: "GymUsers",
                newName: "IX_GymUsers_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_Gym_Sets_ParentRef",
                table: "GymSetEO",
                newName: "IX_GymSetEO_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_Gym_Sessions_ParentRef",
                table: "GymSession",
                newName: "IX_GymSession_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_Gym_Repetitions_ParentRef",
                table: "GymRepetition",
                newName: "IX_GymRepetition_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_Gym_Exercises_ParentRef",
                table: "GymExerciseEO",
                newName: "IX_GymExerciseEO_ParentRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GymUsers",
                table: "GymUsers",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GymSetEO",
                table: "GymSetEO",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GymSession",
                table: "GymSession",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GymRepetition",
                table: "GymRepetition",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GymExerciseEO",
                table: "GymExerciseEO",
                column: "ClassRef");

            migrationBuilder.AddForeignKey(
                name: "FK_GymExerciseEO_GymSession_ParentRef",
                table: "GymExerciseEO",
                column: "ParentRef",
                principalTable: "GymSession",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GymRepetition_GymSetEO_ParentRef",
                table: "GymRepetition",
                column: "ParentRef",
                principalTable: "GymSetEO",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GymSession_GymUsers_ParentRef",
                table: "GymSession",
                column: "ParentRef",
                principalTable: "GymUsers",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GymSetEO_GymExerciseEO_ParentRef",
                table: "GymSetEO",
                column: "ParentRef",
                principalTable: "GymExerciseEO",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GymUsers_AspNetUsers_ParentRef",
                table: "GymUsers",
                column: "ParentRef",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GymExerciseEO_GymSession_ParentRef",
                table: "GymExerciseEO");

            migrationBuilder.DropForeignKey(
                name: "FK_GymRepetition_GymSetEO_ParentRef",
                table: "GymRepetition");

            migrationBuilder.DropForeignKey(
                name: "FK_GymSession_GymUsers_ParentRef",
                table: "GymSession");

            migrationBuilder.DropForeignKey(
                name: "FK_GymSetEO_GymExerciseEO_ParentRef",
                table: "GymSetEO");

            migrationBuilder.DropForeignKey(
                name: "FK_GymUsers_AspNetUsers_ParentRef",
                table: "GymUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GymUsers",
                table: "GymUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GymSetEO",
                table: "GymSetEO");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GymSession",
                table: "GymSession");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GymRepetition",
                table: "GymRepetition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GymExerciseEO",
                table: "GymExerciseEO");

            migrationBuilder.RenameTable(
                name: "GymUsers",
                newName: "Gym_Users");

            migrationBuilder.RenameTable(
                name: "GymSetEO",
                newName: "Gym_Sets");

            migrationBuilder.RenameTable(
                name: "GymSession",
                newName: "Gym_Sessions");

            migrationBuilder.RenameTable(
                name: "GymRepetition",
                newName: "Gym_Repetitions");

            migrationBuilder.RenameTable(
                name: "GymExerciseEO",
                newName: "Gym_Exercises");

            migrationBuilder.RenameIndex(
                name: "IX_GymUsers_ParentRef",
                table: "Gym_Users",
                newName: "IX_Gym_Users_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_GymSetEO_ParentRef",
                table: "Gym_Sets",
                newName: "IX_Gym_Sets_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_GymSession_ParentRef",
                table: "Gym_Sessions",
                newName: "IX_Gym_Sessions_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_GymRepetition_ParentRef",
                table: "Gym_Repetitions",
                newName: "IX_Gym_Repetitions_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_GymExerciseEO_ParentRef",
                table: "Gym_Exercises",
                newName: "IX_Gym_Exercises_ParentRef");

            migrationBuilder.AddColumn<int>(
                name: "WorkoutsThisWeek",
                table: "Gym_Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gym_Users",
                table: "Gym_Users",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gym_Sets",
                table: "Gym_Sets",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gym_Sessions",
                table: "Gym_Sessions",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gym_Repetitions",
                table: "Gym_Repetitions",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gym_Exercises",
                table: "Gym_Exercises",
                column: "ClassRef");

            migrationBuilder.AddForeignKey(
                name: "FK_Gym_Exercises_Gym_Sessions_ParentRef",
                table: "Gym_Exercises",
                column: "ParentRef",
                principalTable: "Gym_Sessions",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gym_Repetitions_Gym_Sets_ParentRef",
                table: "Gym_Repetitions",
                column: "ParentRef",
                principalTable: "Gym_Sets",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gym_Sessions_Gym_Users_ParentRef",
                table: "Gym_Sessions",
                column: "ParentRef",
                principalTable: "Gym_Users",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gym_Sets_Gym_Exercises_ParentRef",
                table: "Gym_Sets",
                column: "ParentRef",
                principalTable: "Gym_Exercises",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gym_Users_AspNetUsers_ParentRef",
                table: "Gym_Users",
                column: "ParentRef",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
