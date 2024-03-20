using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServiceInfrastructure.Migrations
{
    public partial class bt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDay",
                table: "T_Users",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDay",
                table: "T_Users");
        }
    }
}
