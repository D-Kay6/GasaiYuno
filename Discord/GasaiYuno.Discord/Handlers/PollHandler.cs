using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using GasaiYuno.Interface.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Handlers
{
    public class PollHandler : IHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly Func<IUnitOfWork<IPollRepository>> _pollRepository;
        private readonly ILocalization _localization;
        private readonly ILogger<PollHandler> _logger;
        private readonly Timer _timer;

        public PollHandler(DiscordShardedClient client, Func<IUnitOfWork<IPollRepository>> pollRepository, ILocalization localization, ILogger<PollHandler> logger)
        {
            _client = client;
            _pollRepository = pollRepository;
            _localization = localization;
            _logger = logger;

            _timer = new Timer(CheckPolls, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public Task Ready()
        {
            _client.ReactionAdded += OnReactionAdded;
            _client.ChannelDestroyed += OnChannelDestroyed;
            _client.MessageDeleted += OnMessageDeleted;
            _client.MessagesBulkDeleted += OnMessagesBulkDeleted;

            _timer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (channel is not SocketGuildChannel guildChannel) return;

            var repository = _pollRepository();
            var poll = await repository.DataSet.GetAsync(guildChannel.Guild.Id, guildChannel.Id, message.Id);
            if (poll == null || poll.MultiSelect) return;

            var userMessage = await message.DownloadAsync();
            var user = reaction.User.IsSpecified ? reaction.User.Value : guildChannel.Guild.GetUser(reaction.UserId);
            await userMessage.RemoveReactionsAsync(user, userMessage.Reactions.Where(x => !x.Key.Equals(reaction.Emote)).Select(x => x.Key).ToArray());
        }

        private async Task OnChannelDestroyed(SocketChannel channel)
        {
            if (channel is not SocketGuildChannel guildChannel) return;

            var repository = _pollRepository();
            var polls = await repository.DataSet.ListAsync(guildChannel.Guild.Id, guildChannel.Id).ConfigureAwait(false);
            if (!polls.Any()) return;

            await repository.BeginAsync().ConfigureAwait(false);
            repository.DataSet.RemoveRange(polls);
            await repository.SaveAsync().ConfigureAwait(false);
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            if (channel is not SocketGuildChannel guildChannel) return;

            var repository = _pollRepository();
            var poll = await repository.DataSet.GetAsync(guildChannel.Guild.Id, guildChannel.Id, message.Id).ConfigureAwait(false);
            if (poll == null) return;

            await repository.BeginAsync().ConfigureAwait(false);
            repository.DataSet.Remove(poll);
            await repository.SaveAsync().ConfigureAwait(false);
        }

        private async Task OnMessagesBulkDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> messages, ISocketMessageChannel channel)
        {
            if (channel is not SocketGuildChannel guildChannel) return;

            var repository = _pollRepository();
            var polls = await repository.DataSet.ListAsync(guildChannel.Guild.Id, guildChannel.Id).ConfigureAwait(false);
            if (!polls.Any()) return;

            polls = polls.Where(x => messages.Any(y => y.Id == x.Message)).ToList();
            if (!polls.Any()) return;

            await repository.BeginAsync().ConfigureAwait(false);
            repository.DataSet.RemoveRange(polls);
            await repository.SaveAsync().ConfigureAwait(false);
        }

        private async void CheckPolls(object stateInfo)
        {
            var repository = _pollRepository();

            var polls = await repository.DataSet.ListAsync(expired: true).ConfigureAwait(false);
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

                await repository.BeginAsync().ConfigureAwait(false);
                repository.DataSet.RemoveRange(polls);
                await repository.SaveAsync().ConfigureAwait(false);
            }

            _timer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
        }
    }
}