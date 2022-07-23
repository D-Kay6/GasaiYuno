using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Raffles.Mediator.Commands;
using GasaiYuno.Discord.Raffles.Mediator.Requests;
using MediatR;

namespace GasaiYuno.Discord.Raffles.Listeners;

internal class RaffleListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly IMediator _mediator;
    private readonly Timer _timer;
    private readonly Random _random;

    public RaffleListener(DiscordShardedClient client, IMediator mediator)
    {
        _client = client;
        _mediator = mediator;

        _timer = new Timer(CheckRaffles, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _random = new Random();
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

        await _mediator.Publish(new RemoveRafflesCommand(guildChannel.Guild.Id, guildChannel.Id)).ConfigureAwait(false);
    }

    private async Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
    {
        var channel = await cachedChannel.GetOrDownloadAsync().ConfigureAwait(false);
        if (channel is not SocketGuildChannel guildChannel) return;

        await _mediator.Publish(new RemoveRaffleCommand(guildChannel.Guild.Id, guildChannel.Id, cachedMessage.Id)).ConfigureAwait(false);
    }

    private async void CheckRaffles(object stateInfo)
    {
        var raffles = await _mediator.Send(new ListRafflesRequest(true)).ConfigureAwait(false);
        if (raffles.Any())
        {
            foreach (var raffle in raffles)
            {
                var guild = _client.GetGuild(raffle.Server);
                var channel = guild?.GetTextChannel(raffle.Channel);
                if (channel == null) continue;

                var message = await channel.GetMessageAsync(raffle.Message).ConfigureAwait(false);
                SocketGuildUser guildUser = null;
                for (var i = 0; i < 10; i++)
                {
                    var randomIndex = _random.Next(0, raffle.Entries.Count - 1);
                    var userWon = raffle.Entries[randomIndex];
                    guildUser = guild.GetUser(userWon.User);
                    if (guildUser != null) break;
                }
                if (guildUser == null) continue;

                var translation = await _mediator.Send(new GetTranslationRequest(raffle.Server)).ConfigureAwait(false);
                var embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(translation.Message("Automation.Raffle.Result.Title"));
                embedBuilder.AddField(raffle.Title, translation.Message("Automation.Raffle.Result.Winner", guildUser.Mention));
                await channel.SendMessageAsync(embed: embedBuilder.Build(), messageReference: new MessageReference(message.Id, message.Channel.Id));
                await _mediator.Publish(new RemoveRaffleCommand(raffle.Server, raffle.Channel, raffle.Message)).ConfigureAwait(false);
            }
        }

        _timer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
    }

    public ValueTask DisposeAsync()
    {
        return _timer?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}