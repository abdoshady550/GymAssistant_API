using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAssistant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class WorkoutExerciseWorkoutSessionnavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // نظف البيانات الأول
            migrationBuilder.Sql(@"
            UPDATE WorkoutExercises 
            SET ExerciseId = NULL 
            WHERE ExerciseId IS NOT NULL 
            AND ExerciseId NOT IN (SELECT Id FROM Exercises);
            
            UPDATE WorkoutExercises 
            SET UserExerciseId = NULL 
            WHERE UserExerciseId IS NOT NULL 
            AND UserExerciseId NOT IN (SELECT Id FROM UserExercises);
        ");
            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_UserExerciseId",
                table: "WorkoutExercises",
                column: "UserExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_UserExercises_UserExerciseId",
                table: "WorkoutExercises",
                column: "UserExerciseId",
                principalTable: "UserExercises",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_UserExercises_UserExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutExercises_UserExerciseId",
                table: "WorkoutExercises");
        }
    }
}
