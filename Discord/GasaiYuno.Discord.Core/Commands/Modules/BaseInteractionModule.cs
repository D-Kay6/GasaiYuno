using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Domain.Models;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Commands.Modules
{
    public abstract class BaseInteractionModule<TImplementor> : BaseInteractionModule<TImplementor, SocketInteraction> { }

    public abstract class BaseInteractionModule<TImplementor, TInteraction> : InteractionModuleBase<ShardedInteractionContext<TInteraction>> where TInteraction : SocketInteraction
    {
        public InteractiveService Interactivity { get; init; }
        public IMediator Mediator { get; init; }
        public IUnitOfWork UnitOfWork { get; init; }
        public ILogger<TImplementor> Logger { get; init; }

        protected Server Server { get; private set; }
        protected ITranslation Translation { get; set; }

        /// <inheritdoc />
        public override async Task BeforeExecuteAsync(ICommandInfo command)
        {
            Server = await Mediator.Send(new GetServerRequest(Context.Guild.Id, Context.Guild.Name)).ConfigureAwait(false);
            Translation = await Mediator.Send(new GetTranslationRequest(Server.Language)).ConfigureAwait(false);
        }
    }
}