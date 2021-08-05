using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Infrastructure.UnitOfWorks
{
    public class CommandUnitOfWork : UnitOfWork, IUnitOfWork<ICommandRepository>
    {
        public ICommandRepository DataSet { get; }

        /// <summary>
        /// Creates a new <see cref="CommandUnitOfWork"/>
        /// </summary>
        /// <param name="context">The DbContext that will be used.</param>
        /// <param name="repository">The repository that will be used.</param>
        public CommandUnitOfWork(DataContext context, ICommandRepository repository) : base((repository as CommandRepository)?.Context)
        {
            DataSet = repository;
        }
    }
}