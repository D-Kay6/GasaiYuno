using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    public class KickModule : BaseInteractionModule<KickModule>
    {
        [SlashCommand("kick", "Kick a user who misbehaves, or just because you feel like it.", true)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickCommand([Summary(description: "The user to ban.")] IGuildUser user, [Summary(description: "The reason for the ban.")] string reason = null)
        {
            await user.KickAsync(reason).ConfigureAwait(false);

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithThumbnailUrl(user.GetAvatarUrl());
            embedBuilder.AddField(Translation.Message("Moderation.Kick.User"), user.Mention);
            if (!string.IsNullOrWhiteSpace(reason))
                embedBuilder.AddField(Translation.Message("Moderation.Kick.Reason"), reason);

            await RespondAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }
    }
}