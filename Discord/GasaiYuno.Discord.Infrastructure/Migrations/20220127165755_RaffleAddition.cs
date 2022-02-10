using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    public partial class RaffleAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Raffles",
                columns: table => new
                {
                    Channel = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Message = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ServerId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Users = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raffles", x => new { x.ServerId, x.Channel, x.Message });
                    table.ForeignKey(
                        name: "FK_Raffles_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Raffles");
        }
    }
}
