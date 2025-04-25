using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class makeItId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserEmail1",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserEmail2",
                table: "Friendships");

            migrationBuilder.RenameColumn(
                name: "UserEmail2",
                table: "Friendships",
                newName: "UserId2");

            migrationBuilder.RenameColumn(
                name: "UserEmail1",
                table: "Friendships",
                newName: "UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_UserEmail2",
                table: "Friendships",
                newName: "IX_Friendships_UserId2");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_UserEmail1_UserEmail2",
                table: "Friendships",
                newName: "IX_Friendships_UserId1_UserId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId1",
                table: "Friendships",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId2",
                table: "Friendships",
                column: "UserId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId1",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId2",
                table: "Friendships");

            migrationBuilder.RenameColumn(
                name: "UserId2",
                table: "Friendships",
                newName: "UserEmail2");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "Friendships",
                newName: "UserEmail1");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_UserId2",
                table: "Friendships",
                newName: "IX_Friendships_UserEmail2");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_UserId1_UserId2",
                table: "Friendships",
                newName: "IX_Friendships_UserEmail1_UserEmail2");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserEmail1",
                table: "Friendships",
                column: "UserEmail1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserEmail2",
                table: "Friendships",
                column: "UserEmail2",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
