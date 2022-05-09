using Discord.WebSocket;
using GasaiYuno.Discord.Chatbot.Interfaces;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Chatbot.Listeners;

internal class ChatListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly ISessionService _sessionService;
    private readonly IMediator _mediator;
    private readonly ILogger<ChatListener> _logger;

    public ChatListener(DiscordShardedClient client, ISessionService sessionService, IMediator mediator, ILogger<ChatListener> logger)
    {
        _client = client;
        _sessionService = sessionService;
        _mediator = mediator;
        _logger = logger;
    }

    public Task Start()
    {
        _sessionService.SessionCreated += OnSessionCreated;
        _client.MessageReceived += OnMessageReceived;
        _client.ThreadMemberLeft += OnThreadMemberLeft;

        return Task.CompletedTask;
    }

    private Task OnSessionCreated(ISession session)
    {
        session.SessionEnded += OnSessionEnded;
        return Task.CompletedTask;
    }

    private async Task OnSessionEnded(ISession session)
    {
        if (_client.GetChannel(session.Thread.Id) is not SocketThreadChannel thread) return;

        var translation = await _mediator.Send(new GetTranslationRequest(thread.Guild.Id)).ConfigureAwait(false);
        if (session.TimedOut)
            await thread.SendMessageAsync(translation.Message("Entertainment.Chat.Timeout")).ConfigureAwait(false);

        await thread.ModifyAsync(x =>
        {
            x.Archived = true;
            x.Locked = true;
        }).ConfigureAwait(false);
    }

    private async Task OnMessageReceived(SocketMessage message)
    {
        if (message.Channel is not SocketThreadChannel thread) return;

        var session = _sessionService.GetSession($"{thread.Guild.Id}_{message.Author.Id}");
        if (session == null) return;
        if (message.Author.Id != session.UserId) return;

        var response = await session.GetResponseAsync(message.Content).ConfigureAwait(false);
        await thread.SendMessageAsync(response.Message).ConfigureAwait(false);
    }

    private Task OnThreadMemberLeft(SocketThreadUser user)
    {
        var session = _sessionService.GetSession($"{user.Thread.Guild.Id}_{user.Id}");
        session?.Dispose();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}