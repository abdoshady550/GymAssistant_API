using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAssistant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserLinkedDeviceToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_DeviceTokens_AppUsers_UserId",
                table: "DeviceTokens",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceTokens_AppUsers_UserId",
                table: "DeviceTokens");
        }
    }
}
