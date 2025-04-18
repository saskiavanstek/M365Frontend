using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M365TrainingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TrainingSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantLinks",
                table: "TrainingSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionCounter",
                table: "TrainingSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "ParticipantLinks",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "SessionCounter",
                table: "TrainingSessions");
        }
    }
}
