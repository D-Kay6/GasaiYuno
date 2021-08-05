using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    public partial class PollAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Polls",
                columns: table => new
                {
                    Channel = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Message = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ServerId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    MultiSelect = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polls", x => new { x.ServerId, x.Channel, x.Message });
                    table.ForeignKey(
                        name: "FK_Polls_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Polls");
        }
    }
}
