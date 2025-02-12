using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmirsCorrespondence.Migrations
{
    /// <inheritdoc />
    public partial class SeedadminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[] { 1, "Admin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "IsActive", "PasswordHash", "RoleId", "UserName", "UsersUserId", "createdAt" },
                values: new object[] { 1, "admin@corp.com", true, "Dahmylarey", 1, "admin", null, new DateTime(2025, 2, 2, 15, 0, 33, 932, DateTimeKind.Local).AddTicks(7150) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1);
        }
    }
}
