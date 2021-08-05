using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Infrastructure.UnitOfWorks
{
    public class DynamicChannelUnitOfWork : UnitOfWork, IUnitOfWork<IDynamicChannelRepository>
    {
        public IDynamicChannelRepository DataSet { get; }

        /// <summary>
        /// Creates a new <see cref="DynamicChannelUnitOfWork"/>
        /// </summary>
        /// <param name="context">The DbContext that will be used.</param>
        /// <param name="repository">The repository that will be used.</param>
        public DynamicChannelUnitOfWork(DataContext context, IDynamicChannelRepository repository) : base((repository as DynamicChannelRepository)?.Context)
        {
            DataSet = repository;
        }
    }
}