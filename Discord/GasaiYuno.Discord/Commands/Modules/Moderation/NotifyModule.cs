using Discord;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    [Group("Notify")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class NotifyModule : BaseModule<NotifyModule>
    {
        [Command]
        [Priority(-1)]
        public async Task AnnounceDefaultAsync([Remainder] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                await ReplyAsync(Translation.Message("Moderation.Notify.Invalid.Message"));
                return;
            }

            await SendAnnouncementAsync(Context.Guild.Users, message, Context.Guild.Name);
        }

        [Command("Global")]
        [RequireOwner]
        public async Task AnnounceGlobalAsync([Remainder] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                await ReplyAsync(Translation.Message("Moderation.Notify.Invalid.Message"));
                return;
            }

            var users = new Dictionary<ulong, IUser>();
            foreach (var guild in Context.Client.Guilds)
            {
                if (guild?.Owner == null || guild.Id == 264445053596991498) continue;
                if (users.ContainsKey(guild.OwnerId)) continue;
                users.Add(guild.OwnerId, guild.Owner);
            }

            await SendAnnouncementAsync(users.Values, message, "Update notice");
        }

        private async Task SendAnnouncementAsync(IReadOnlyCollection<IUser> users, string message, string title = "")
        {
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(title);
            embedBuilder.WithDescription(message);
            var embed = embedBuilder.Build();
            foreach (var user in users)
            {
                if (user.IsBot) continue;
                try
                {
                    var channel = await user.GetOrCreateDMChannelAsync().ConfigureAwait(false);
                    if (channel == null) continue;
                    
                    await channel.SendMessageAsync(embed: embed);
                    await Task.Delay(500);
                }
                catch (HttpException e)
                {
                    Logger.LogError(e, "Unable to send notification {Message} to user {User}", message, user);
                }
            }
        }
    }
}