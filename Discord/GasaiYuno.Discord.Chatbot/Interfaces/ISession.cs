using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Chatbot.Interfaces
{
    public interface ISession : IDisposable
    {
        event Func<ISession, Task> SessionEnded;
        
        /// <summary>
        /// If the session has timed out.
        /// </summary>
        bool TimedOut { get; }

        /// <summary>
        /// The id of this session.
        /// </summary>
        string SessionId { get; }

        /// <summary>
        /// The id of the user this session is linked to.
        /// </summary>
        ulong UserId { get; }

        /// <summary>
        /// The thread this session is linked to.
        /// </summary>
        SocketThreadChannel Thread { get; }

        /// <summary>
        /// Get the response to the provided message asynchronously.
        /// </summary>
        /// <param name="message">The message to respond to.</param>
        /// <returns>The response to the message.</returns>
        Task<IResponse> GetResponseAsync(string message);
    }
}