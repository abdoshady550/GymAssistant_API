using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAssistant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPredefinedWorkoutEntitiesAndLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Sections",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Sections",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Sections",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Sections",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Sections",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Exercises",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Exercises",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Exercises",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentAr",
                table: "Exercises",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipmentEn",
                table: "Exercises",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructionsAr",
                table: "Exercises",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructionsEn",
                table: "Exercises",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Exercises",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Exercises",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "AppUsers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "en");

            migrationBuilder.CreateTable(
                name: "PredefinedWorkdays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DayNumber = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredefinedWorkdays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PredefinedWorkoutSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PredefinedWorkdayId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: true),
                    EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredefinedWorkoutSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredefinedWorkoutSessions_PredefinedWorkdays_PredefinedWorkdayId",
                        column: x => x.PredefinedWorkdayId,
                        principalTable: "PredefinedWorkdays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PredefinedSessionExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PredefinedWorkoutSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DefaultSets = table.Column<int>(type: "int", nullable: false),
                    DefaultReps = table.Column<int>(type: "int", nullable: false),
                    DefaultRestTimeSeconds = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredefinedSessionExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredefinedSessionExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PredefinedSessionExercises_PredefinedWorkoutSessions_PredefinedWorkoutSessionId",
                        column: x => x.PredefinedWorkoutSessionId,
                        principalTable: "PredefinedWorkoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PredefinedSessionExercises_ExerciseId",
                table: "PredefinedSessionExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_PredefinedSessionExercises_PredefinedWorkoutSessionId",
                table: "PredefinedSessionExercises",
                column: "PredefinedWorkoutSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PredefinedWorkoutSessions_PredefinedWorkdayId",
                table: "PredefinedWorkoutSessions",
                column: "PredefinedWorkdayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PredefinedSessionExercises");

            migrationBuilder.DropTable(
                name: "PredefinedWorkoutSessions");

            migrationBuilder.DropTable(
                name: "PredefinedWorkdays");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "EquipmentAr",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "EquipmentEn",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "InstructionsAr",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "InstructionsEn",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "AppUsers");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
