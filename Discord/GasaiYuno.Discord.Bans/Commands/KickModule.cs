using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands;

namespace GasaiYuno.Discord.Bans.Commands;

[EnabledInDm(false)]
public class KickModule : BaseInteractionModule<KickModule>
{
    [DefaultMemberPermissions(GuildPermission.KickMembers)]
    [RequireUserPermission(GuildPermission.KickMembers)]
    [SlashCommand("kick", "Kick a user who misbehaves, or just because you feel like it.")]
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