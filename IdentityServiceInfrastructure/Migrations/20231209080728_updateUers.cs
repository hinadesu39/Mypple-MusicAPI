using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServiceInfrastructure.Migrations
{
    public partial class updateUers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "T_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserAvatar",
                table: "T_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "T_Users");

            migrationBuilder.DropColumn(
                name: "UserAvatar",
                table: "T_Users");
        }
    }
}
