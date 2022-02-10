using Discord.Commands;
using GasaiYuno.Discord.Core.Mediator.Events;
using GasaiYuno.Discord.Music.Listeners;
using MediatR;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Music.Mediator.Events
{
    internal sealed class ClientReadyEventHandler : INotificationHandler<ClientReadyEvent>
    {
        private readonly MusicListener _musicListener;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        public ClientReadyEventHandler(MusicListener musicListener, CommandService commandService, IServiceProvider serviceProvider)
        {
            _musicListener = musicListener;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(ClientReadyEvent notification, CancellationToken cancellationToken)
        {
            await _musicListener.Start();
            await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        }
    }
}