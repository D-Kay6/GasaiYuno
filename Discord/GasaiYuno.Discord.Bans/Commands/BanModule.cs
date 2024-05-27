using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Bans.Mediator.Commands;
using GasaiYuno.Discord.Bans.Models;
using GasaiYuno.Discord.Core.Commands;

namespace GasaiYuno.Discord.Bans.Commands;

[EnabledInDm(false)]
public class BanModule : BaseInteractionModule<BanModule>
{
    [DefaultMemberPermissions(GuildPermission.BanMembers)]
    [RequireUserPermission(GuildPermission.BanMembers)]
    [SlashCommand("ban", "Ban a user who misbehaves, or just because you feel like it.")]
    public async Task BanCommand(
        [Summary(description: "The user to ban.")] IGuildUser user,
        [Summary(description: "How long to ban the user for.")] TimeSpan? duration = null,
        [Summary(description: "The reason for the ban.")] string reason = null,
        [Summary(description: "Until how many days back the messages of the user should be removed.")] int days = 0)
    {
        var endDate = duration.HasValue ? DateTime.Now + duration : null;
        if (endDate.HasValue)
        {
            var ban = new Ban
            {
                Server = Context.Guild.Id,
                User = user.Id,
                EndDate = endDate.Value,
                Reason = reason
            };
            await Mediator.Publish(new AddBanCommand(ban)).ConfigureAwait(false);
        }

        await user.BanAsync(days, reason).ConfigureAwait(false);
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithThumbnailUrl(user.GetAvatarUrl());
        embedBuilder.AddField(Localization.Translate("Moderation.Ban.User"), user.Mention);
        embedBuilder.AddField(Localization.Translate("Moderation.Ban.Duration"), endDate?.ToString("g") ?? Localization.Translate("Generic.Forever"));
        embedBuilder.AddField(Localization.Translate("Moderation.Ban.Reason"), !string.IsNullOrEmpty(reason) ? reason : Localization.Translate("Generic.None"));

        await RespondAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
    }
}