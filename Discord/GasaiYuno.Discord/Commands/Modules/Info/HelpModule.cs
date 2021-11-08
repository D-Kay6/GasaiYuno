using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Extensions;
using Interactivity;
using Interactivity.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Info
{
    [Group("Help")]
    [Alias("Info", "Command", "Commands")]
    public class HelpModule : BaseModule<HelpModule>
    {
        [Command]
        [Priority(-1)]
        public Task HelpDefaultAsync()
        {
            var isAdmin = (Context.User as SocketGuildUser)?.GuildPermissions.Administrator ?? false;
            var pages = new List<PageBuilder>();

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Info.Help.Entertainment.Title"));
            embedBuilder.WithDescription(Translation.Message("Info.Help.Entertainment.Message"));
            embedBuilder.AddField(Translation.Message("Info.Help.Entertainment.Birthday.Title"), Translation.Message("Info.Help.Entertainment.Birthday.Normal", Server.Prefix));
            embedBuilder.AddField(Translation.Message("Info.Help.Entertainment.Chat.Title"), Translation.Message("Info.Help.Entertainment.Chat.Normal", Server.Prefix));
            embedBuilder.AddField(Translation.Message("Info.Help.Entertainment.Kill.Title"), Translation.Message("Info.Help.Entertainment.Kill.Normal", Server.Prefix));
            embedBuilder.AddField(Translation.Message("Info.Help.Entertainment.CustomCommand.Title"), Translation.Message("Info.Help.Entertainment.CustomCommand.Normal", Server.Prefix) + (isAdmin ? Environment.NewLine + Translation.Message("Info.Help.Entertainment.CustomCommand.Admin", Server.Prefix) : string.Empty));
            embedBuilder.AddField(Translation.Message("Info.Help.Entertainment.Music1.Title"), Translation.Message("Info.Help.Entertainment.Music1.Normal", Server.Prefix));
            embedBuilder.AddField(Translation.Message("Info.Help.Entertainment.Music2.Title"), Translation.Message("Info.Help.Entertainment.Music2.Normal", Server.Prefix) + (isAdmin ? Environment.NewLine + Translation.Message("Info.Help.Entertainment.Music2.Admin", Server.Prefix) : string.Empty));
            pages.Add(PageBuilder.FromEmbed(embedBuilder.Build()));

            if (isAdmin)
            {
                embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(Translation.Message("Info.Help.Moderation.Title"));
                embedBuilder.WithDescription(Translation.Message("Info.Help.Moderation.Message"));
                embedBuilder.AddField(Translation.Message("Info.Help.Moderation.Prefix.Title"), Translation.Message("Info.Help.Moderation.Prefix.Admin", Server.Prefix));
                embedBuilder.AddField(Translation.Message("Info.Help.Moderation.Language.Title"), Translation.Message("Info.Help.Moderation.Language.Admin", Server.Prefix));
                embedBuilder.AddField(Translation.Message("Info.Help.Moderation.Kick.Title"), Translation.Message("Info.Help.Moderation.Kick.Admin", Server.Prefix));
                embedBuilder.AddField(Translation.Message("Info.Help.Moderation.Ban.Title"), Translation.Message("Info.Help.Moderation.Ban.Admin", Server.Prefix));
                pages.Add(PageBuilder.FromEmbed(embedBuilder.Build()));

                embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(Translation.Message("Info.Help.Automation.Title"));
                embedBuilder.WithDescription(Translation.Message("Info.Help.Automation.Message"));
                embedBuilder.AddField(Translation.Message("Info.Help.Automation.DynamicChannel.Title"), Translation.Message("Info.Help.Automation.DynamicChannel.Admin", Server.Prefix));
                pages.Add(PageBuilder.FromEmbed(embedBuilder.Build()));

                embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(Translation.Message("Info.Help.Notification.Title"));
                embedBuilder.WithDescription(Translation.Message("Info.Help.Notification.Message"));
                embedBuilder.AddField(Translation.Message("Info.Help.Notification.Welcome.Title"), Translation.Message("Info.Help.Notification.Welcome.Admin", Server.Prefix));
                pages.Add(PageBuilder.FromEmbed(embedBuilder.Build()));
            }

            embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Info.Help.Info.Title"));
            embedBuilder.WithDescription(Translation.Message("Info.Help.Info.Message"));
            embedBuilder.AddField(Translation.Message("Info.Help.Info.Help.Title"), Translation.Message("Info.Help.Info.Help.Normal", Server.Prefix));
            embedBuilder.AddField(Translation.Message("Info.Help.Info.Invite.Title"), Translation.Message("Info.Help.Info.Invite.Normal", Server.Prefix));
            embedBuilder.AddField(Translation.Message("Info.Help.Info.Support.Title"), Translation.Message("Info.Help.Info.Support.Normal", Server.Prefix));
            pages.Add(PageBuilder.FromEmbed(embedBuilder.Build()));

            embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Info.Help.Note.Title"));
            embedBuilder.WithDescription(Translation.Message("Info.Help.Note.Message", Server.Prefix));
            pages.Add(PageBuilder.FromEmbed(embedBuilder.Build()));

            var paginator = new StaticPaginatorBuilder()
                .WithUsers(Context.User)
                .WithPages(pages)
                .WithFooter(PaginatorFooter.PageNumber | PaginatorFooter.Users)
                .WithDefaultEmotes()
                .Build();

            return Interactivity.SendPaginatorComponentAsync(Context.Client, paginator, Context.Channel, TimeSpan.FromMinutes(2));
        }

        [Command]
        [Priority(-1)]
        public Task HelpDefaultAsync([Remainder] string message) => ReplyAsync(Translation.Message("Info.Help.Unknown"));

        [Command("CustomCommands")]
        [Alias("cc", "CustomCommand")]
        public async Task HelpCustomCommandAsync()
        {
            var isAdmin = (Context.User as SocketGuildUser)?.GuildPermissions.Administrator ?? false;

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Info.Help.Entertainment.CustomCommand.Title"));
            embedBuilder.WithDescription(Translation.Message("Info.Help.Entertainment.CustomCommand.Normal", Server.Prefix) + (isAdmin ? Environment.NewLine + Translation.Message("Info.Help.Entertainment.CustomCommand.Admin", Server.Prefix) : string.Empty));
            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }

        [Command("Music")]
        [Alias("m")]
        public async Task HelpMusicAsync()
        {
            var isAdmin = (Context.User as SocketGuildUser)?.GuildPermissions.Administrator ?? false;

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Info.Help.Entertainment.Music.Title"));
            embedBuilder.WithDescription(Translation.Message("Info.Help.Entertainment.Music.Normal", Server.Prefix) + (isAdmin ? Environment.NewLine + Translation.Message("Info.Help.Entertainment.Music.Admin", Server.Prefix) : string.Empty));
            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }

        [Command("DynamicChannels")]
        [Alias("dc", "DynamicChannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task HelpDynamicChannelAsync()
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Info.Help.Automation.DynamicChannel.Title"));
            embedBuilder.WithDescription(Translation.Message("Info.Help.Automation.DynamicChannel.Admin", Server.Prefix));
            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }

        [Command("Welcome")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task HelpWelcomeAsync()
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(Translation.Message("Info.Help.Notification.Welcome.Title"));
            embedBuilder.WithDescription(Translation.Message("Info.Help.Notification.Welcome.Admin", Server.Prefix));
            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }
    }
}