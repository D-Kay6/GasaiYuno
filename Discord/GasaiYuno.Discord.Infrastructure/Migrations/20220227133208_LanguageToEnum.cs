using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    public partial class LanguageToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_Languages_LanguageId",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Servers_LanguageId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Servers");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Servers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Servers");

            migrationBuilder.CreateSequence(
                name: "languageseq",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LocalizedName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Servers",
                type: "int",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Servers_LanguageId",
                table: "Servers",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_Languages_LanguageId",
                table: "Servers",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id");
        }
    }
}
