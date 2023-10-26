using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicInfrastructure.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title_TitleTrans",
                table: "T_Music");

            migrationBuilder.DropColumn(
                name: "Title_TitleTrans",
                table: "T_Album");

            migrationBuilder.RenameColumn(
                name: "Title_Title",
                table: "T_Music",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Title_Title",
                table: "T_Album",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "T_Music",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "T_Album",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "T_Music",
                newName: "Title_Title");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "T_Album",
                newName: "Title_Title");

            migrationBuilder.AlterColumn<string>(
                name: "Title_Title",
                table: "T_Music",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Title_TitleTrans",
                table: "T_Music",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title_Title",
                table: "T_Album",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Title_TitleTrans",
                table: "T_Album",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
