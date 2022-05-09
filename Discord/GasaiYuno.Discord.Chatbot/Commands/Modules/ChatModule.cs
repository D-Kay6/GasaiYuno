using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GasaiYuno.Discord.Chatbot.Interfaces;
using GasaiYuno.Discord.Core.Commands.Modules;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Chatbot.Commands.Modules;

[EnabledInDm(false)]
[Group("chat", "Engage in a casual, but probably still weird, conversation with me.")]
public class ChatModule : BaseInteractionModule<ChatModule>
{
    private string SessionId => $"{Context.Guild.Id}_{Context.User.Id}";

    private readonly ISessionService _sessionService;

    public ChatModule(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [SlashCommand("start", "Engage in a casual, but probably still weird, conversation with me.")]
    public async Task ChatDefaultAsync([Summary("message", "The message to start the conversation with.")] string input = null)
    {
        var session = _sessionService.GetSession(SessionId);
        if (session != null)
        {
            await RespondAsync(Translation.Message("Entertainment.Chat.Invalid.Exists", Server.Prefix), ephemeral: true).ConfigureAwait(false);
            return;
        }

        await RespondAsync(Translation.Message("Entertainment.Chat.Thread"), ephemeral: true).ConfigureAwait(false);
        var thread = await ((Context.Channel as SocketTextChannel)!).CreateThreadAsync($"Chat with {Context.User.Username}", ThreadType.PublicThread, ThreadArchiveDuration.OneHour)
            .ConfigureAwait(false);
        await thread.AddUserAsync(Context.User as SocketGuildUser).ConfigureAwait(false);
        session = _sessionService.CreateSession(SessionId, Context.User.Id, thread);
        if (!string.IsNullOrWhiteSpace(input))
        {
            var response = await session.GetResponseAsync(input).ConfigureAwait(false);
            await thread.SendMessageAsync(response.Message).ConfigureAwait(false);
        }
        else
        {
            await thread.SendMessageAsync(Translation.Message("Entertainment.Chat.Start")).ConfigureAwait(false);
        }
    }

    [SlashCommand("stop", "End the ongoing conversation.")]
    public async Task ChatStopAsync()
    {
        var session = _sessionService.GetSession(SessionId);
        if (session == null)
        {
            await RespondAsync(Translation.Message("Entertainment.Chat.Invalid.None"), ephemeral: true).ConfigureAwait(false);
            return;
        }

        await RespondAsync(Translation.Message("Entertainment.Chat.End"), ephemeral: true).ConfigureAwait(false);
        session.Dispose();
    }
}