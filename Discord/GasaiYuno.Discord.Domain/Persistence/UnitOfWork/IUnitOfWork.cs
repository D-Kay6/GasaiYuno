using GasaiYuno.Discord.Domain.Persistence.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Domain.Persistence.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IBanRepository Bans { get; }
    ICommandRepository Commands { get; }
    INotificationRepository Notifications { get; }
    IStickyMessageRepository StickyMessages { get; }
    IDynamicChannelRepository DynamicChannels { get; }
    IGameRoleRepository GameRoles { get; }
    IDistributionRoleRepository DistributionRoles { get; }
    IPollRepository Polls { get; }
    IRaffleRepository Raffles { get; }
    IServerRepository Servers { get; }

    /// <summary>
    /// Save all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}