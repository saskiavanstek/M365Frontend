using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M365TrainingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddKlantIdToTrainingSessionsAndParticipantsLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParticipantLinks",
                table: "TrainingSessions");

            migrationBuilder.CreateTable(
                name: "Klanten",
                columns: table => new
                {
                    KlantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KlantNaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KlantDetails = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klanten", x => x.KlantId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Klanten");

            migrationBuilder.AddColumn<string>(
                name: "ParticipantLinks",
                table: "TrainingSessions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
