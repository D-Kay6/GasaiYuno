using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    [Group("Kick")]
    [RequireUserPermission(GuildPermission.KickMembers)]
    public class KickModule : BaseModule<KickModule>
    {
        [Command]
        public Task KickDefaultAsync() => ReplyAsync(Translation.Message("Moderation.Kick.Default", Server.Prefix));

        [Command]
        [Priority(-1)]
        public Task KickUserAsync([Remainder] string name) => ReplyAsync(Translation.Message("Generic.Invalid.User", name));

        [Command]
        public async Task KickUserAsync(SocketGuildUser user, [Remainder] string reason = null)
        {
            await user.KickAsync(reason);

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithThumbnailUrl(user.GetAvatarUrl());
            embedBuilder.AddField(Translation.Message("Moderation.Kick.Info.User"), user.Mention);
            if (!string.IsNullOrWhiteSpace(reason))
                embedBuilder.AddField(Translation.Message("Moderation.Kick.Info.Reason"), reason);

            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}