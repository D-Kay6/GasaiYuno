using GasaiYuno.Discord.Chatbot.Interfaces;
using GasaiYuno.Discord.Chatbot.Models;
using System;
using System.Collections.Concurrent;

namespace GasaiYuno.Discord.Chatbot.Services
{
    internal class SessionService : IChatService
    {
        private readonly ConcurrentDictionary<string, ISession> _sessions;

        private readonly EndPoint _endPoint;
        private readonly TimeSpan _sessionDuration;

        /// <summary>
        /// Creates a new instance of <see cref="Session"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the api key is not valid.</exception>
        public SessionService(string apiKey, double idleDuration)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("Can't connect without a API key.");
            
            _sessions = new ConcurrentDictionary<string, ISession>();
            _endPoint = new EndPoint(apiKey);
            _sessionDuration = TimeSpan.FromSeconds(idleDuration);
        }

        /// <inheritdoc/>
        public ISession GetSession(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) 
                throw new ArgumentNullException(nameof(sessionId));

            if (!_sessions.TryGetValue(sessionId, out var session))
                session = null;

            return session;
        }

        /// <inheritdoc/>
        public ISession CreateSession(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentNullException(nameof(sessionId));

            if (_sessions.TryGetValue(sessionId, out var session))
                throw new InvalidOperationException("The session id is already in use.");

            return _sessions.GetOrAdd(sessionId, _ => NewSession(sessionId));
        }

        /// <inheritdoc/>
        public ISession GetOrCreateSession(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) 
                throw new ArgumentNullException(nameof(sessionId));

            return _sessions.GetOrAdd(sessionId, _ => NewSession(sessionId));
        }

        private Session NewSession(string sessionId)
        {
            var session = new Session(sessionId, _endPoint, _sessionDuration);
            session.OnSessionEnded += SessionEnded;
            return session;
        }

        private void SessionEnded(object sender, EventArgs e)
        {
            if (sender is not Session session) return;

            _sessions.TryRemove(session.SessionId, out var removedSession);
        }
    }
}