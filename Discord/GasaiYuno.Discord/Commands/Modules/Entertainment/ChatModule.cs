using Discord.Commands;
using GasaiYuno.Interface.Chatbot;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Entertainment
{
    [Group("Chat")]
    public class ChatModule : BaseModule<ChatModule>
    {
        private string SessionId => $"{Context.Guild.Id}_{Context.User.Id}";

        private readonly IChatService _chatService;

        public ChatModule(IChatService chatService)
        {
            _chatService = chatService;
        }

        [Command]
        public async Task ChatDefaultAsync()
        {
            var session = _chatService.GetSession(SessionId);
            if (session != null)
            {
                await ReplyAsync(Translation.Message("Entertainment.Chat.Invalid.Exists", Server.Prefix)).ConfigureAwait(false);
                return;
            }

            session = _chatService.CreateSession(SessionId);
            await ReplyAsync(Translation.Message("Entertainment.Chat.Start")).ConfigureAwait(false);

            while (session.Active)
            {
                var userMessage = await Interactivity.NextMessageAsync(x => x.Author.Id == Context.User.Id, timeout: TimeSpan.FromMinutes(2)).ConfigureAwait(false);
                if (userMessage is not {IsSuccess: true})
                {
                    await ReplyAsync(Translation.Message("Entertainment.Chat.Timeout")).ConfigureAwait(false);
                    session.Dispose();
                    return;
                }

                if (userMessage.Value.Content.StartsWith(Server.Prefix, StringComparison.OrdinalIgnoreCase) || userMessage.Value.Content.StartsWith(Context.Client.CurrentUser.Mention, StringComparison.OrdinalIgnoreCase))
                {
                    if (userMessage.Value.Content.EndsWith("chat stop", StringComparison.OrdinalIgnoreCase))
                        return;

                    continue;
                }

                var response = await session.GetResponseAsync(userMessage.Value.Content).ConfigureAwait(false);
                await ReplyAsync(response.Message);
            }

            session.Dispose();
            await ReplyAsync(Translation.Message("Entertainment.Chat.End")).ConfigureAwait(false);
        }

        [Command("Stop")]
        public async Task ChatStopAsync()
        {
            var session = _chatService.GetSession(SessionId);
            if (session == null)
            {
                await ReplyAsync(Translation.Message("Entertainment.Chat.Invalid.None")).ConfigureAwait(false);
                return;
            }

            session.Dispose();
            await ReplyAsync(Translation.Message("Entertainment.Chat.End")).ConfigureAwait(false);
        }
    }
}