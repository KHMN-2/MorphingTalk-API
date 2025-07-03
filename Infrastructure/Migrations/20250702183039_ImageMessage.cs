using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImageMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConversationUsers_ConversationId_UserId",
                table: "ConversationUsers");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ConversationUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationUsers_ConversationId_UserId",
                table: "ConversationUsers",
                columns: new[] { "ConversationId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConversationUsers_ConversationId_UserId",
                table: "ConversationUsers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ConversationUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConversationUsers_ConversationId_UserId",
                table: "ConversationUsers",
                columns: new[] { "ConversationId", "UserId" },
                unique: true);
        }
    }
}
