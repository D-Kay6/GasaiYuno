using Discord.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Events
{
    public class CustomCommandEventHandler : INotificationHandler<CustomCommandEvent>
    {
        private readonly CommandService _commandService;
        private readonly IMediator _mediator;
        private readonly ILogger<CustomCommandEventHandler> _logger;

        public CustomCommandEventHandler(CommandService commandService, IMediator mediator, ILogger<CustomCommandEventHandler> logger)
        {
            _commandService = commandService;
            _mediator = mediator;
            _logger = logger;
        }

        public Task Handle(CustomCommandEvent request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}