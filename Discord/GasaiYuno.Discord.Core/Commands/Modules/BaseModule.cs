using Discord.Commands;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Domain;
using Interactivity;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Commands.Modules
{
    public abstract class BaseModule<TImplementor> : BaseModule
    {
        public ILogger<TImplementor> Logger { get; init; }
    }

    public abstract class BaseModule : ModuleBase<ShardedCommandContext>
    {
        public InteractivityService Interactivity { get; init; }

        public IMediator Mediator { get; init; }

        protected Server Server { get; private set; }
        protected ITranslation Translation { get; set; }
        
        protected override void BeforeExecute(CommandInfo command)
        {
            Task.WaitAll(Prepare());
            base.BeforeExecute(command);
        }

        protected virtual async Task Prepare()
        {
            Server = await Mediator.Send(new GetServerRequest(Context.Guild.Id, Context.Guild.Name)).ConfigureAwait(false);
            Translation = await Mediator.Send(new GetTranslationRequest(Server?.Language?.Name)).ConfigureAwait(false);
        }
    }
}