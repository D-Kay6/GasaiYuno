using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;

namespace GasaiYuno.Discord.Infrastructure.UnitOfWorks
{
    public class NotificationUnitOfWork : UnitOfWork, IUnitOfWork<INotificationRepository>
    {
        public INotificationRepository DataSet { get; }

        /// <summary>
        /// Creates a new <see cref="NotificationUnitOfWork"/>
        /// </summary>
        /// <param name="context">The DbContext that will be used.</param>
        /// <param name="repository">The repository that will be used.</param>
        public NotificationUnitOfWork(DataContext context, INotificationRepository repository) : base((repository as NotificationRepository)?.Context)
        {
            DataSet = repository;
        }
    }
}