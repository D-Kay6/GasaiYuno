using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    public partial class PollCommandSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Polls",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "MultiSelect",
                table: "Polls");

            migrationBuilder.AddColumn<decimal>(
                name: "Id",
                table: "Polls",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Selections",
                table: "Polls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Polls",
                table: "Polls",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Polls_ServerId",
                table: "Polls",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Polls",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Polls_ServerId",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "Selections",
                table: "Polls");

            migrationBuilder.AddColumn<bool>(
                name: "MultiSelect",
                table: "Polls",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Polls",
                table: "Polls",
                columns: new[] { "ServerId", "Channel", "Message" });
        }
    }
}
