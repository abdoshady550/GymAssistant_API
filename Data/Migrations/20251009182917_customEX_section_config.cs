using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAssistant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class customEX_section_config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DifficultyLevel",
                table: "UserExercises",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "UserExercises",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("50505050-5050-5050-5050-505050505050"));

            migrationBuilder.CreateIndex(
                name: "IX_UserExercises_SectionId",
                table: "UserExercises",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserExercises_Sections_SectionId",
                table: "UserExercises",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserExercises_Sections_SectionId",
                table: "UserExercises");

            migrationBuilder.DropIndex(
                name: "IX_UserExercises_SectionId",
                table: "UserExercises");

            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "UserExercises");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "UserExercises");
        }
    }
}
