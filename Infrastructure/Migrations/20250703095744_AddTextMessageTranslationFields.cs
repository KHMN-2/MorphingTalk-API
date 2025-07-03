using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTextMessageTranslationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TranslatedContent",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TranslatedContents",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VoiceMessage_IsTranslated",
                table: "Messages",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslatedContent",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "TranslatedContents",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "VoiceMessage_IsTranslated",
                table: "Messages");
        }
    }
}
