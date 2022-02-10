using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Chatbot.Interfaces
{
    public interface ISession : IDisposable
    {
        /// <summary>
        /// The state of the session.
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// Get the response to the provided message.
        /// </summary>
        /// <param name="message">The message to respond to.</param>
        /// <returns>The response to the message.</returns>
        IResponse GetResponse(string message);

        /// <summary>
        /// Get the response to the provided message asynchronously.
        /// </summary>
        /// <param name="message">The message to respond to.</param>
        /// <returns>The response to the message.</returns>
        Task<IResponse> GetResponseAsync(string message);
    }
}