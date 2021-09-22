using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    [Group("Ban")]
    [RequireUserPermission(GuildPermission.BanMembers)]
    public class BanModule : BaseModule<BanModule>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BanModule(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Command]
        public Task BanDefaultAsync() => ReplyAsync(Translation.Message("Moderation.Ban.Default"));

        [Priority(-1)]
        [Command]
        public Task BanUserAsync([Remainder] string name) => ReplyAsync(Translation.Message("Generic.Invalid.User", name));

        [Command]
        public async Task BanUserAsync(SocketGuildUser user)
        {
            await user.BanAsync().ConfigureAwait(false);

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithThumbnailUrl(user.GetAvatarUrl());
            embedBuilder.AddField(Translation.Message("Moderation.Ban.Info.User"), user.Mention);
            embedBuilder.AddField(Translation.Message("Moderation.Ban.Info.Duration"), Translation.Message("Generic.Forever"));
            embedBuilder.AddField(Translation.Message("Moderation.Ban.Info.Reason"), Translation.Message("Generic.None"));

            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }

        [Command]
        [Priority(-1)]
        public Task BanUserAsync(SocketGuildUser user, [Remainder] string message) => BanUserAsync(user, TimeSpan.Zero, message);

        [Command]
        public async Task BanUserAsync(SocketGuildUser user, TimeSpan duration, [Remainder] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                await BanUserAsync(user).ConfigureAwait(false);
                return;
            }

            var parts = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var endDate = duration > TimeSpan.Zero ? DateTime.Now + duration : (DateTime?)null;
            var time = Translation.Message("Generic.Forever");
            if (endDate != null)
            {
                time = endDate.Value.ToString("g");
                parts = parts.Skip(1).ToArray();
            }

            var reason = string.Join(' ', parts);
            if (endDate != null)
            {
                var ban = new Ban
                {
                    Server = Server,
                    User = user.Id,
                    EndDate = endDate.Value,
                    Reason = reason
                };
                
                _unitOfWork.Bans.Add(ban);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            await user.BanAsync(0, reason).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(reason))
                reason = Translation.Message("Generic.None");

            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithThumbnailUrl(user.GetAvatarUrl());
            embedBuilder.AddField(Translation.Message("Moderation.Ban.Info.User"), user.Mention);
            embedBuilder.AddField(Translation.Message("Moderation.Ban.Info.Duration"), time);
            embedBuilder.AddField(Translation.Message("Moderation.Ban.Info.Reason"), reason);

            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }
    }
}