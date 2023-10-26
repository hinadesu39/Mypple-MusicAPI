using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicInfrastructure.Migrations
{
    public partial class upload0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Album",
                table: "T_Music",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ArtistId",
                table: "T_Music",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "PublishTime",
                table: "T_Music",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Artist",
                table: "T_Album",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Album",
                table: "T_Music");

            migrationBuilder.DropColumn(
                name: "ArtistId",
                table: "T_Music");

            migrationBuilder.DropColumn(
                name: "PublishTime",
                table: "T_Music");

            migrationBuilder.DropColumn(
                name: "Artist",
                table: "T_Album");
        }
    }
}
