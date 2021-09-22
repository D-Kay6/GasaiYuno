using Discord.WebSocket;
using MediatR;
using System.Runtime.Serialization;

namespace GasaiYuno.Discord.Mediator.Events
{
    public class CommandStartedEvent : INotification
    {
        public int ArgumentPosition { get; init; }
        public SocketUserMessage Message { get; init; }
        public SocketGuildChannel Channel { get; init; }

        public CommandStartedEvent(int argumentPosition, SocketUserMessage message, SocketGuildChannel channel)
        {
            ArgumentPosition = argumentPosition;
            Message = message;
            Channel = channel;
        }
    }
}