using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DurationToSeconds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceDuration",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "Messages",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "Messages");

            migrationBuilder.AddColumn<double>(
                name: "VoiceDuration",
                table: "Messages",
                type: "float",
                nullable: true);
        }
    }
}
