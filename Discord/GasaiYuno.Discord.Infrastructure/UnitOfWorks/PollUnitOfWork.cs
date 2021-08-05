using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Infrastructure.UnitOfWorks
{
    public class PollUnitOfWork : UnitOfWork, IUnitOfWork<IPollRepository>
    {
        public IPollRepository DataSet { get; }

        /// <summary>
        /// Creates a new <see cref="PollUnitOfWork"/>
        /// </summary>
        /// <param name="context">The DbContext that will be used.</param>
        /// <param name="repository">The repository that will be used.</param>
        public PollUnitOfWork(DataContext context, IPollRepository repository) : base((repository as PollRepository)?.Context)
        {
            DataSet = repository;
        }
    }
}