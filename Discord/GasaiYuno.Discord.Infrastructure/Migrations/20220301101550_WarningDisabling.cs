using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    public partial class WarningDisabling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WarningDisabled",
                table: "Servers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarningDisabled",
                table: "Servers");
        }
    }
}
