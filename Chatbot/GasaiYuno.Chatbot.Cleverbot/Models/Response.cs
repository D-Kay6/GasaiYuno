using GasaiYuno.Interface.Chatbot;

namespace GasaiYuno.Chatbot.Cleverbot.Models
{
    public struct Response : IResponse
    {
        /// <inheritdoc/>
        public string Input { get; init; }

        /// <inheritdoc/>
        public string Message { get; init; }
    }
}