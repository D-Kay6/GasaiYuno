using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Chatbot.Interfaces;

public interface ISessionService
{
    event Func<ISession, Task> SessionCreated;

    /// <summary>
    /// Get an active chat session.
    /// </summary>
    /// <param name="sessionId">The id of the session.</param>
    /// <returns><see cref="ISession"/></returns>
    ISession GetSession(string sessionId);

    /// <summary>
    /// Create a new chat session.
    /// </summary>
    /// <param name="sessionId">The id of the chat session.</param>
    /// <param name="userId">The id of the user.</param>
    /// <param name="thread">The thread for the chat.</param>
    /// <returns><see cref="ISession"/></returns>
    ISession CreateSession(string sessionId, ulong userId, SocketThreadChannel thread);

    /// <summary>
    /// Get an active chat session.
    /// If none is found, a new one is created.
    /// </summary>
    /// <param name="sessionId">The id of the chat session.</param>
    /// <param name="userId">The id of the user.</param>
    /// <param name="thread">The thread for the chat.</param>
    /// <returns><see cref="ISession"/></returns>
    ISession GetOrCreateSession(string sessionId, ulong userId, SocketThreadChannel thread);
}