using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAssistant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SectionGroupId",
                table: "UserExercises",
                type: "uniqueidentifier",
                                        nullable: true);


            migrationBuilder.AddColumn<Guid>(
                name: "SectionGroupId",
                table: "Exercises",
                type: "uniqueidentifier",
                             nullable: true);


            migrationBuilder.CreateTable(
                name: "SectionGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SectionGroups_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserExercises_SectionGroupId",
                table: "UserExercises",
                column: "SectionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_SectionGroupId",
                table: "Exercises",
                column: "SectionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroups_SectionId",
                table: "SectionGroups",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_SectionGroups_SectionGroupId",
                table: "Exercises",
                column: "SectionGroupId",
                principalTable: "SectionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserExercises_SectionGroups_SectionGroupId",
                table: "UserExercises",
                column: "SectionGroupId",
                principalTable: "SectionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_SectionGroups_SectionGroupId",
                table: "Exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_UserExercises_SectionGroups_SectionGroupId",
                table: "UserExercises");

            migrationBuilder.DropTable(
                name: "SectionGroups");

            migrationBuilder.DropIndex(
                name: "IX_UserExercises_SectionGroupId",
                table: "UserExercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_SectionGroupId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "SectionGroupId",
                table: "UserExercises");

            migrationBuilder.DropColumn(
                name: "SectionGroupId",
                table: "Exercises");
        }
    }
}
