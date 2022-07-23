using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Polls.Mediator.Commands;
using GasaiYuno.Discord.Polls.Mediator.Requests;
using MediatR;

namespace GasaiYuno.Discord.Polls.Listeners;

internal class PollListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly IMediator _mediator;
    private readonly Timer _timer;

    public PollListener(DiscordShardedClient client, IMediator mediator)
    {
        _client = client;
        _mediator = mediator;

        _timer = new Timer(CheckPolls, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public Task Start()
    {
        _client.ChannelDestroyed += OnChannelDestroyed;
        _client.MessageDeleted += OnMessageDeleted;

        _timer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
        return Task.CompletedTask;
    }

    private async Task OnChannelDestroyed(SocketChannel channel)
    {
        if (channel is not SocketGuildChannel guildChannel) return;
        
        await _mediator.Publish(new RemovePollsCommand(guildChannel.Guild.Id, guildChannel.Id)).ConfigureAwait(false);
    }

    private async Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
    {
        var channel = await cachedChannel.GetOrDownloadAsync().ConfigureAwait(false);
        if (channel is not SocketGuildChannel guildChannel) return;

        await _mediator.Publish(new RemovePollCommand(guildChannel.Guild.Id, guildChannel.Id, cachedMessage.Id)).ConfigureAwait(false);
    }

    private async void CheckPolls(object stateInfo)
    {
        var polls = await _mediator.Send(new ListPollsRequest(true)).ConfigureAwait(false);
        if (polls.Any())
        {
            foreach (var poll in polls)
            {
                var guild = _client.GetGuild(poll.Server);
                var channel = guild?.GetTextChannel(poll.Channel);
                if (channel == null) continue;

                var translation = await _mediator.Send(new GetTranslationRequest(poll.Server)).ConfigureAwait(false);
                var totalScore = poll.Selections.Select(x => x.SelectedOption).GroupBy(x => x).ToList();
                var maxScore = totalScore.MaxBy(x => x.Count())?.Key ?? -1;
                var highestIndexes = totalScore.Where(x => x.Count() == maxScore).Select(x => x.Key).ToList();
                var options = highestIndexes.Select(index => poll.Options[index]).ToList();
                var selectedOptions = options.Any() ? string.Join("\n", options) : translation.Message("Automation.Poll.Result.None");

                var message = await channel.GetMessageAsync(poll.Message).ConfigureAwait(false);
                var embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(translation.Message("Automation.Poll.Result.Title"));
                embedBuilder.AddField(poll.Text, selectedOptions);
                await channel.SendMessageAsync(embed: embedBuilder.Build(), messageReference: new MessageReference(message.Id, message.Channel.Id));
                await _mediator.Publish(new RemovePollCommand(poll.Server, poll.Channel, poll.Message)).ConfigureAwait(false);
            }
        }

        _timer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
    }

    public ValueTask DisposeAsync()
    {
        return _timer?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}