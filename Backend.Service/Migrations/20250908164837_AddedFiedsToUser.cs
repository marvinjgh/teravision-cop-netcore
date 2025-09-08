using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddedFiedsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResfreshTokenExpiryTime",
                table: "User",
                newName: "RefreshTokenExpiryTime");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiryTime",
                table: "User",
                newName: "ResfreshTokenExpiryTime");
        }
    }
}
