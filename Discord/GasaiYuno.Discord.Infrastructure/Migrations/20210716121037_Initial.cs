using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "!"),
                    LanguageId = table.Column<int>(type: "int", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servers_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bans",
                columns: table => new
                {
                    User = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ServerId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => new { x.ServerId, x.User });
                    table.ForeignKey(
                        name: "FK_Bans_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomCommands",
                columns: table => new
                {
                    Command = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ServerId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomCommands", x => new { x.ServerId, x.Command });
                    table.ForeignKey(
                        name: "FK_CustomCommands_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DynamicChannels",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServerId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenerationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "-- channel"),
                    Channels = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneratedChannels = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicChannels", x => new { x.ServerId, x.Name });
                    table.ForeignKey(
                        name: "FK_DynamicChannels_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServerId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false, defaultValue: "Welcome to the party {0}. Hope you will have a good time with us."),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Channel = table.Column<decimal>(type: "decimal(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => new { x.ServerId, x.Type });
                    table.ForeignKey(
                        name: "FK_Notifications_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Servers_LanguageId",
                table: "Servers",
                column: "LanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bans");

            migrationBuilder.DropTable(
                name: "CustomCommands");

            migrationBuilder.DropTable(
                name: "DynamicChannels");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropSequence(
                name: "languageseq");
        }
    }
}
