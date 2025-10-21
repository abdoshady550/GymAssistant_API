using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAssistant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class profile_Group_config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClientProfileId",
                table: "SectionGroups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroups_ClientProfileId",
                table: "SectionGroups",
                column: "ClientProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionGroups_ClientProfiles_ClientProfileId",
                table: "SectionGroups",
                column: "ClientProfileId",
                principalTable: "ClientProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionGroups_ClientProfiles_ClientProfileId",
                table: "SectionGroups");

            migrationBuilder.DropIndex(
                name: "IX_SectionGroups_ClientProfileId",
                table: "SectionGroups");

            migrationBuilder.DropColumn(
                name: "ClientProfileId",
                table: "SectionGroups");
        }
    }
}
