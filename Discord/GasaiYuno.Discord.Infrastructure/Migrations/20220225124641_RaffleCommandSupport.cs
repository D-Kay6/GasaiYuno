using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    public partial class RaffleCommandSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Raffles",
                table: "Raffles");

            migrationBuilder.AddColumn<decimal>(
                name: "Id",
                table: "Raffles",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Raffles",
                table: "Raffles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Raffles_ServerId",
                table: "Raffles",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Raffles",
                table: "Raffles");

            migrationBuilder.DropIndex(
                name: "IX_Raffles_ServerId",
                table: "Raffles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Raffles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Raffles",
                table: "Raffles",
                columns: new[] { "ServerId", "Channel", "Message" });
        }
    }
}
