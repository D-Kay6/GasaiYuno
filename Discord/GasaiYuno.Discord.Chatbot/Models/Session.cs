using Discord.WebSocket;
using GasaiYuno.Discord.Chatbot.Interfaces;

namespace GasaiYuno.Discord.Chatbot.Models;

internal class Session : ISession
{
    public event Func<ISession, Task> SessionEnded;

    /// <inheritdoc/>
    public bool TimedOut { get; private set; }

    /// <inheritdoc/>
    public string SessionId { get; }

    /// <inheritdoc/>
    public ulong UserId { get; private set; }

    /// <inheritdoc/>
    public SocketThreadChannel Thread { get; private set; }

    private readonly EndPoint _endPoint;
    private readonly TimeSpan _idleDuration;
    private readonly Timer _timer;

    private bool _active;
    private string _state;

    /// <summary>
    /// Creates a new instance of <see cref="Session"/>.
    /// </summary>
    /// <param name="sessionId">The id of this session.</param>
    /// <param name="endPoint">The endpoint to connect to.</param>
    /// <param name="idleDuration">The maximum duration the session may remain idle.</param>
    public Session(string sessionId, EndPoint endPoint, TimeSpan idleDuration)
    {
        SessionId = sessionId;

        _endPoint = endPoint;
        _idleDuration = idleDuration;

        _timer = new Timer(ThresholdPassed, null, idleDuration, Timeout.InfiniteTimeSpan);
        _active = true;
    }
        
    public void Link(ulong userId, SocketThreadChannel thread)
    {
        if (UserId > 0 || Thread != null)
            throw new InvalidOperationException("The session has already been linked.");
            
        UserId = userId;
        Thread = thread;
    }

    /// <inheritdoc/>
    public async Task<IResponse> GetResponseAsync(string message)
    {
        if (!_active) 
            throw new InvalidOperationException("The session is not active.");

        // In case the reply could take long enough for the timer to elapse.
        _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        var reply = await _endPoint.GetReplyAsync(message, _state).ConfigureAwait(false);
        _state = reply.State;
        _timer.Change(_idleDuration, Timeout.InfiniteTimeSpan);

        return new Response
        {
            Input = reply.InputMessage,
            Message = reply.OutputMessage
        };
    }

    private void ThresholdPassed(object stateInfo)
    {
        TimedOut = true;
        Dispose();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _timer?.Dispose();

        _active = false;
        SessionEnded?.Invoke(this);
    }
}