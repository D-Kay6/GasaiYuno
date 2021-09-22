using Microsoft.EntityFrameworkCore.Migrations;

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    public partial class IncreaseCharLimit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "Servers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "!",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "!");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Servers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Polls",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "Welcome to the party {0}. Hope you will have a good time with us.",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldDefaultValue: "Welcome to the party {0}. Hope you will have a good time with us.");

            migrationBuilder.AlterColumn<string>(
                name: "GenerationName",
                table: "DynamicChannels",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "-- channel",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldDefaultValue: "-- channel");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DynamicChannels",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "CustomCommands",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Command",
                table: "CustomCommands",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "Servers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "!",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "!");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Servers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Polls",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "Welcome to the party {0}. Hope you will have a good time with us.",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldDefaultValue: "Welcome to the party {0}. Hope you will have a good time with us.");

            migrationBuilder.AlterColumn<string>(
                name: "GenerationName",
                table: "DynamicChannels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "-- channel",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldDefaultValue: "-- channel");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "DynamicChannels",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "CustomCommands",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Command",
                table: "CustomCommands",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
