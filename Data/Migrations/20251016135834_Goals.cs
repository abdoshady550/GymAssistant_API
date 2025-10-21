using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAssistant_API.Data.Migrations
{
    /// <inheritdoc />
    public partial class Goals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "WeightKg",
                table: "BodyMeasurements",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "BodyFatGoal",
                table: "BodyMeasurements",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MuscleMassGoal",
                table: "BodyMeasurements",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WeightGoal",
                table: "BodyMeasurements",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyFatGoal",
                table: "BodyMeasurements");

            migrationBuilder.DropColumn(
                name: "MuscleMassGoal",
                table: "BodyMeasurements");

            migrationBuilder.DropColumn(
                name: "WeightGoal",
                table: "BodyMeasurements");

            migrationBuilder.AlterColumn<decimal>(
                name: "WeightKg",
                table: "BodyMeasurements",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
