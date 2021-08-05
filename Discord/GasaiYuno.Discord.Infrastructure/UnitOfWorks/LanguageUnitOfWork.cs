using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Infrastructure.UnitOfWorks
{
    public class LanguageUnitOfWork : UnitOfWork, IUnitOfWork<ILanguageRepository>
    {
        public ILanguageRepository DataSet { get; }

        /// <summary>
        /// Creates a new <see cref="LanguageUnitOfWork"/>
        /// </summary>
        /// <param name="context">The DbContext that will be used.</param>
        /// <param name="repository">The repository that will be used.</param>
        public LanguageUnitOfWork(DataContext context, ILanguageRepository repository) : base((repository as LanguageRepository)?.Context)
        {
            DataSet = repository;
        }
    }
}