using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVoiceModelIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserVoiceModel_VoiceModelId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserVoiceModel",
                table: "UserVoiceModel");

            migrationBuilder.RenameTable(
                name: "UserVoiceModel",
                newName: "UserVoiceModels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserVoiceModels",
                table: "UserVoiceModels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserVoiceModels_VoiceModelId",
                table: "AspNetUsers",
                column: "VoiceModelId",
                principalTable: "UserVoiceModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserVoiceModels_VoiceModelId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserVoiceModels",
                table: "UserVoiceModels");

            migrationBuilder.RenameTable(
                name: "UserVoiceModels",
                newName: "UserVoiceModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserVoiceModel",
                table: "UserVoiceModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserVoiceModel_VoiceModelId",
                table: "AspNetUsers",
                column: "VoiceModelId",
                principalTable: "UserVoiceModel",
                principalColumn: "Id");
        }
    }
}
