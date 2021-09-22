using MediatR;
using System.Runtime.Serialization;

namespace GasaiYuno.Discord.Mediator.Events
{
    public class CustomCommandEvent : INotification
    {
        public ulong Id { get; init; }
        public string Name { get; init; }
    }
}