using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Infrastructure.UnitOfWorks
{
    public class BanUnitOfWork : UnitOfWork, IUnitOfWork<IBanRepository>
    {
        public IBanRepository DataSet { get; }

        /// <summary>
        /// Creates a new <see cref="BanUnitOfWork"/>
        /// </summary>
        /// <param name="context">The DbContext that will be used.</param>
        /// <param name="repository">The repository that will be used.</param>
        public BanUnitOfWork(DataContext context, IBanRepository repository) : base((repository as BanRepository)?.Context)
        {
            DataSet = repository;
        }
    }
}