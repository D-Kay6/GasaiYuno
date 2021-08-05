using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Infrastructure.UnitOfWorks
{
    public class DynamicRoleUnitOfWork : UnitOfWork, IUnitOfWork<IDynamicRoleRepository>
    {
        public IDynamicRoleRepository DataSet { get; }

        /// <summary>
        /// Creates a new <see cref="DynamicRoleUnitOfWork"/>
        /// </summary>
        /// <param name="context">The DbContext that will be used.</param>
        /// <param name="repository">The repository that will be used.</param>
        public DynamicRoleUnitOfWork(DataContext context, IDynamicRoleRepository repository) : base((repository as DynamicRoleRepository)?.Context)
        {
            DataSet = repository;
        }
    }
}