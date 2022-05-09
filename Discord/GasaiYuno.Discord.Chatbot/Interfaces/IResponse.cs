namespace GasaiYuno.Discord.Chatbot.Interfaces;

public interface IResponse
{
    /// <summary>
    /// The message responded to.
    /// </summary>
    string Input { get; }

    /// <summary>
    /// The response message.
    /// </summary>
    string Message { get; }
}