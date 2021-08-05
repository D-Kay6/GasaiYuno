namespace GasaiYuno.Interface.Chatbot
{
    public interface IChatService
    {
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
        /// <returns><see cref="ISession"/></returns>
        ISession CreateSession(string sessionId);

        /// <summary>
        /// Get an active chat session.
        /// If none is found, a new one is created.
        /// </summary>
        /// <param name="sessionId">The id of the chat session.</param>
        /// <returns><see cref="ISession"/></returns>
        ISession GetOrCreateSession(string sessionId);
    }
}