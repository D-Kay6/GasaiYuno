using GasaiYuno.Interface.Chatbot;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Chatbot.Cleverbot.Models
{
    internal class Session : ISession
    {
        public event EventHandler SessionEnded;

        /// <inheritdoc/>
        public bool Active { get; private set; }
        public string SessionId { get; }

        private readonly EndPoint _endPoint;
        private readonly TimeSpan _idleDuration;
        private readonly Timer _timer;

        private string _state;

        /// <summary>
        /// Creates a new instance of <see cref="Session"/>.
        /// </summary>
        /// <param name="sessionId">The id of this session.</param>
        /// <param name="endPoint">The endpoint to connect to.</param>
        /// <param name="idleDuration">The maximum duration the session may remain idle.</param>
        public Session(string sessionId, EndPoint endPoint, TimeSpan idleDuration)
        {
            Active = true;
            SessionId = sessionId;

            _endPoint = endPoint;
            _idleDuration = idleDuration;

            _timer = new Timer(ThresholdPassed, null, idleDuration, Timeout.InfiniteTimeSpan);
        }

        /// <inheritdoc/>
        public IResponse GetResponse(string message) => GetResponseAsync(message).GetAwaiter().GetResult();

        /// <inheritdoc/>
        public async Task<IResponse> GetResponseAsync(string message)
        {
            if (!Active) 
                throw new InvalidOperationException("The session is not active.");
            
            var reply = await _endPoint.GetReplyAsync(message, _state).ConfigureAwait(false);
            _state = reply.State;
            _timer.Change(_idleDuration, Timeout.InfiniteTimeSpan);

            return new Response
            {
                Input = reply.InputMessage,
                Message = reply.OutputMessage
            };
        }

        private void ThresholdPassed(object stateInfo) => Dispose();

        /// <inheritdoc/>
        public void Dispose()
        {
            _timer?.Dispose();

            Active = false;
            SessionEnded?.Invoke(this, EventArgs.Empty);
        }
    }
}