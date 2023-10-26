using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicInfrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Album",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PicUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title_Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title_TitleTrans = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishTime = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Album", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "T_Artist",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PicUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Artist", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "T_Music",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AudioUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PicUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title_Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title_TitleTrans = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Duration = table.Column<double>(type: "float", nullable: false),
                    Artist = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lyric = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsLiked = table.Column<bool>(type: "bit", nullable: false),
                    PlayTimes = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Music", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_Album_ArtistId_IsDeleted",
                table: "T_Album",
                columns: new[] { "ArtistId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Artist_Name_Id",
                table: "T_Artist",
                columns: new[] { "Name", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_T_Music_AlbumId_IsDeleted",
                table: "T_Music",
                columns: new[] { "AlbumId", "IsDeleted" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Album");

            migrationBuilder.DropTable(
                name: "T_Artist");

            migrationBuilder.DropTable(
                name: "T_Music");
        }
    }
}
