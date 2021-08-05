using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Infrastructure.UnitOfWorks
{
    public class ServerUnitOfWork : UnitOfWork, IUnitOfWork<IServerRepository>
    {
        public IServerRepository DataSet { get; }

        /// <summary>
        /// Creates a new <see cref="ServerUnitOfWork"/>
        /// </summary>
        /// <param name="context">The DbContext that will be used.</param>
        /// <param name="repository">The repository that will be used.</param>
        public ServerUnitOfWork(DataContext context, IServerRepository repository) : base((repository as ServerRepository)?.Context)
        {
            DataSet = repository;
        }
    }
}