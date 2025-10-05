using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAssistant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerRequestSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainerRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TraineeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RespondedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerRequests_ClientProfiles_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "ClientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainerRequests_ClientProfiles_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "ClientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerRequests_CreatedAtUtc",
                table: "TrainerRequests",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerRequests_TraineeId",
                table: "TrainerRequests",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerRequests_TrainerId_TraineeId_Status",
                table: "TrainerRequests",
                columns: new[] { "TrainerId", "TraineeId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainerRequests");
        }
    }
}
