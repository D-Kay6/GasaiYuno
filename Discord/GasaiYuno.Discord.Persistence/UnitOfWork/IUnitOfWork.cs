using GasaiYuno.Discord.Persistence.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Persistence.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBanRepository Bans { get; }
        ICommandRepository Commands { get; }
        IDynamicChannelRepository DynamicChannels { get; }
        IDynamicRoleRepository DynamicRoles { get; }
        ILanguageRepository Languages { get; }
        INotificationRepository Notifications { get; }
        IPollRepository Polls { get; }
        IServerRepository Servers { get; }

        /// <summary>
        /// Save all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}