using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    public partial class ConcurrencyFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Users",
                table: "Raffles");

            migrationBuilder.DropColumn(
                name: "Options",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "Selections",
                table: "Polls");

            migrationBuilder.CreateSequence(
                name: "raffleentryseq",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "PollOptions",
                columns: table => new
                {
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PollId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollOptions", x => new { x.PollId, x.Value });
                    table.ForeignKey(
                        name: "FK_PollOptions_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollSelections",
                columns: table => new
                {
                    User = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    SelectedOption = table.Column<int>(type: "int", nullable: false),
                    PollId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollSelections", x => new { x.PollId, x.User, x.SelectedOption });
                    table.ForeignKey(
                        name: "FK_PollSelections_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RaffleEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    User = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    RaffleId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaffleEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaffleEntries_Raffles_RaffleId",
                        column: x => x.RaffleId,
                        principalTable: "Raffles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RaffleEntries_RaffleId",
                table: "RaffleEntries",
                column: "RaffleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollOptions");

            migrationBuilder.DropTable(
                name: "PollSelections");

            migrationBuilder.DropTable(
                name: "RaffleEntries");

            migrationBuilder.DropSequence(
                name: "raffleentryseq");

            migrationBuilder.AddColumn<string>(
                name: "Users",
                table: "Raffles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Options",
                table: "Polls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Selections",
                table: "Polls",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
