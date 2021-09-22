using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Events
{
    public class CommandExecutedEventHandler : INotificationHandler<CommandExecutedEvent>
    {
        public async Task Handle(CommandExecutedEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}