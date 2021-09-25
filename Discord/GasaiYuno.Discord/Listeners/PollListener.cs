using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using GasaiYuno.Interface.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    internal class PollListener : IDisposable
    {
        private readonly DiscordShardedClient _client;
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;
        private readonly ILocalization _localization;
        private readonly ILogger<PollListener> _logger;
        private readonly Timer _timer;

        public PollListener(Connection connection, Func<IUnitOfWork> unitOfWorkFactory, ILocalization localization, ILogger<PollListener> logger)
        {
            _client = connection.Client;
            _unitOfWorkFactory = unitOfWorkFactory;
            _localization = localization;
            _logger = logger;

            _timer = new Timer(CheckPolls, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            connection.Ready += OnReady;
        }

        private Task OnReady()
        {
            _client.ReactionAdded += OnReactionAdded;
            _client.ChannelDestroyed += OnChannelDestroyed;
            _client.MessageDeleted += OnMessageDeleted;

            _timer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            var channel = await cachedChannel.GetOrDownloadAsync().ConfigureAwait(false);
            if (channel is not SocketGuildChannel guildChannel) return;

            var unitOfWork = _unitOfWorkFactory();
            var poll = await unitOfWork.Polls.GetAsync(guildChannel.Guild.Id, guildChannel.Id, cachedMessage.Id);
            if (poll == null || poll.MultiSelect) return;

            var userMessage = await cachedMessage.DownloadAsync();
            var user = reaction.User.IsSpecified ? reaction.User.Value : guildChannel.Guild.GetUser(reaction.UserId);
            await userMessage.RemoveReactionsAsync(user, userMessage.Reactions.Where(x => !x.Key.Equals(reaction.Emote)).Select(x => x.Key).ToArray());
        }

        private async Task OnChannelDestroyed(SocketChannel channel)
        {
            if (channel is not SocketGuildChannel guildChannel) return;

            var unitOfWork = _unitOfWorkFactory();
            var polls = await unitOfWork.Polls.ListAsync(guildChannel.Guild.Id, guildChannel.Id).ConfigureAwait(false);
            if (!polls.Any()) return;
            
            unitOfWork.Polls.RemoveRange(polls);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            var channel = await cachedChannel.GetOrDownloadAsync().ConfigureAwait(false);
            if (channel is not SocketGuildChannel guildChannel) return;

            var unitOfWork = _unitOfWorkFactory();
            var poll = await unitOfWork.Polls.GetAsync(guildChannel.Guild.Id, guildChannel.Id, cachedMessage.Id).ConfigureAwait(false);
            if (poll == null) return;
            
            unitOfWork.Polls.Remove(poll);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        private async void CheckPolls(object stateInfo)
        {
            var unitOfWork = _unitOfWorkFactory();

            var polls = await unitOfWork.Polls.ListAsync(expired: true).ConfigureAwait(false);
            if (polls.Any())
            {
                foreach (var poll in polls)
                {
                    var guild = _client.GetGuild(poll.Server.Id);
                    var channel = guild?.GetTextChannel(poll.Channel);
                    if (channel == null) continue;

                    var message = await channel.GetMessageAsync(poll.Message).ConfigureAwait(false);
                    if (message is not IUserMessage userMessage) continue;

                    var highestCount = userMessage.Reactions.Max(x => x.Value.ReactionCount);
                    var selectedEmojis = userMessage.Reactions.Where(x => x.Value.ReactionCount == highestCount).Select(x => x.Key.Name).ToList();

                    var translation = _localization.GetTranslation(poll.Server.Language.Name);
                    var embedBuilder = new EmbedBuilder();
                    embedBuilder.WithTitle(poll.Text);
                    embedBuilder.WithDescription(string.Join(Environment.NewLine, poll.Options.Select(x => $"{(selectedEmojis.Contains(x.Emote) ? "\u2714" : "\u274C")} {x.Message}")));
                    embedBuilder.WithFooter(translation.Message("Automation.Poll.Ended"));
                    embedBuilder.WithTimestamp(DateTimeOffset.Now);
                    var embed = embedBuilder.Build();
                    await userMessage.ModifyAsync(x => x.Embed = embed).ConfigureAwait(false);

                    embedBuilder = new EmbedBuilder();
                    embedBuilder.WithTitle(translation.Message("Automation.Poll.Result.Title"));
                    embedBuilder.AddField(poll.Text, string.Join(Environment.NewLine, poll.Options.Where(x => selectedEmojis.Contains(x.Emote)).Select(x => x.Message)));
                    await channel.SendMessageAsync(embed: embedBuilder.Build(), messageReference: new MessageReference(message.Id, message.Channel.Id));
                }
                
                unitOfWork.Polls.RemoveRange(polls);
                await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            _timer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}