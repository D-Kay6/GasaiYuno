using GasaiYuno.Discord.Chatbot.Interfaces;

namespace GasaiYuno.Discord.Chatbot.Models
{
    public struct Response : IResponse
    {
        /// <inheritdoc/>
        public string Input { get; init; }

        /// <inheritdoc/>
        public string Message { get; init; }
    }
}