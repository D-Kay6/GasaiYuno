using Discord;
using Discord.Rest;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.ExtNamePP.Mediator.Commands;
using GasaiYuno.Discord.ExtNamePP.Mediator.Requests;
using GasaiYuno.Discord.ExtNamePP.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GasaiYuno.Discord.ExtNamePP.Listeners;

public class ExtNamePSListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly IMediator _mediator;
    private readonly ILogger<ExtNamePSListener> _logger;
    private readonly List<ulong> _channelCache;

    public ExtNamePSListener(DiscordShardedClient client, IMediator mediator, ILogger<ExtNamePSListener> logger)
    {
        _client = client;
        _mediator = mediator;
        _logger = logger;
        _channelCache = new List<ulong>();
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}