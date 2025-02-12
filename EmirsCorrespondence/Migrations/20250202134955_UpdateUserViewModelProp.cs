using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmirsCorrespondence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserViewModelProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsersUserId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UsersUserId",
                table: "Users",
                column: "UsersUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UsersUserId",
                table: "Users",
                column: "UsersUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UsersUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UsersUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UsersUserId",
                table: "Users");
        }
    }
}
