using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetUp.Migrations
{
    /// <inheritdoc />
    public partial class TableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GymExerciseEO_GymSessionEO_ParentRef",
                table: "GymExerciseEO");

            migrationBuilder.DropForeignKey(
                name: "FK_GymRepetitionEO_GymSetEO_ParentRef",
                table: "GymRepetitionEO");

            migrationBuilder.DropForeignKey(
                name: "FK_GymSessionEO_GymUsers_ParentRef",
                table: "GymSessionEO");

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
                name: "PK_GymSessionEO",
                table: "GymSessionEO");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GymRepetitionEO",
                table: "GymRepetitionEO");

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
                name: "GymSessionEO",
                newName: "Gym_Sessions");

            migrationBuilder.RenameTable(
                name: "GymRepetitionEO",
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
                name: "IX_GymSessionEO_ParentRef",
                table: "Gym_Sessions",
                newName: "IX_Gym_Sessions_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_GymRepetitionEO_ParentRef",
                table: "Gym_Repetitions",
                newName: "IX_Gym_Repetitions_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_GymExerciseEO_ParentRef",
                table: "Gym_Exercises",
                newName: "IX_Gym_Exercises_ParentRef");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameTable(
                name: "Gym_Users",
                newName: "GymUsers");

            migrationBuilder.RenameTable(
                name: "Gym_Sets",
                newName: "GymSetEO");

            migrationBuilder.RenameTable(
                name: "Gym_Sessions",
                newName: "GymSessionEO");

            migrationBuilder.RenameTable(
                name: "Gym_Repetitions",
                newName: "GymRepetitionEO");

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
                table: "GymSessionEO",
                newName: "IX_GymSessionEO_ParentRef");

            migrationBuilder.RenameIndex(
                name: "IX_Gym_Repetitions_ParentRef",
                table: "GymRepetitionEO",
                newName: "IX_GymRepetitionEO_ParentRef");

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
                name: "PK_GymSessionEO",
                table: "GymSessionEO",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GymRepetitionEO",
                table: "GymRepetitionEO",
                column: "ClassRef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GymExerciseEO",
                table: "GymExerciseEO",
                column: "ClassRef");

            migrationBuilder.AddForeignKey(
                name: "FK_GymExerciseEO_GymSessionEO_ParentRef",
                table: "GymExerciseEO",
                column: "ParentRef",
                principalTable: "GymSessionEO",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GymRepetitionEO_GymSetEO_ParentRef",
                table: "GymRepetitionEO",
                column: "ParentRef",
                principalTable: "GymSetEO",
                principalColumn: "ClassRef",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GymSessionEO_GymUsers_ParentRef",
                table: "GymSessionEO",
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
    }
}
