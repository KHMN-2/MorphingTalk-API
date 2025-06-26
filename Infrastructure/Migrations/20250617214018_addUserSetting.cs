using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addUserSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TranslateMessages",
                table: "ConversationUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseRobotVoice",
                table: "ConversationUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "muteNotifications",
                table: "ConversationUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MuteNotifications",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TranslateMessages",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseRobotVoice",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslateMessages",
                table: "ConversationUsers");

            migrationBuilder.DropColumn(
                name: "UseRobotVoice",
                table: "ConversationUsers");

            migrationBuilder.DropColumn(
                name: "muteNotifications",
                table: "ConversationUsers");

            migrationBuilder.DropColumn(
                name: "MuteNotifications",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TranslateMessages",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UseRobotVoice",
                table: "AspNetUsers");
        }
    }
}
