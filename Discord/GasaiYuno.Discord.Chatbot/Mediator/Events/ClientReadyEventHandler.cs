using Discord.Commands;
using GasaiYuno.Discord.Core.Mediator.Events;
using MediatR;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Chatbot.Mediator.Events
{
    internal sealed class ClientReadyEventHandler : INotificationHandler<ClientReadyEvent>
    {
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        public ClientReadyEventHandler(CommandService commandService, IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(ClientReadyEvent notification, CancellationToken cancellationToken)
        {
            await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        }
    }
}