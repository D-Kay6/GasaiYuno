using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Domain.Models;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    public class BanModule : BaseInteractionModule<BanModule>
    {
        [SlashCommand("ban", "Ban a user who misbehaves, or just because you feel like it.", true)]
        [RequireUserPermission(GuildPermission.BanMembers)]
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
                    Server = Server,
                    User = user.Id,
                    EndDate = endDate.Value,
                    Reason = reason
                };

                UnitOfWork.Bans.Add(ban);
                await UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            await user.BanAsync(days, reason).ConfigureAwait(false);
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithThumbnailUrl(user.GetAvatarUrl());
            embedBuilder.AddField(Translation.Message("Moderation.Ban.User"), user.Mention);
            embedBuilder.AddField(Translation.Message("Moderation.Ban.Duration"), endDate?.ToString("g") ?? Translation.Message("Generic.Forever"));
            embedBuilder.AddField(Translation.Message("Moderation.Ban.Reason"), !string.IsNullOrEmpty(reason) ? reason : Translation.Message("Generic.None"));

            await RespondAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }
    }
}