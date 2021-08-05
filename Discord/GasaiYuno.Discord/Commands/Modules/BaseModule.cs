using Discord.Commands;
using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using GasaiYuno.Interface.Localization;
using Interactivity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules
{
    public abstract class BaseModule<TImplementor> : BaseModule
    {
        public ILogger<TImplementor> Logger { get; init; }
    }

    public abstract class BaseModule : ModuleBase<ShardedCommandContext>
    {
        public InteractivityService Interactivity { get; init; }

        public IUnitOfWork<IServerRepository> ServerRepository { get; init; }
        public ILocalization Localization { get; init; }

        protected Server Server { get; private set; }
        protected ITranslation Translation { get; set; }

        protected override void BeforeExecute(CommandInfo command)
        {
            Task.WaitAll(Prepare());
            base.BeforeExecute(command);
        }

        protected virtual async Task Prepare()
        {
            Server = await ServerRepository.DataSet.GetOrAddAsync(Context.Guild.Id, Context.Guild.Name).ConfigureAwait(false);
            Translation = Localization.GetTranslation(Server.Language?.Name);
        }
    }
}