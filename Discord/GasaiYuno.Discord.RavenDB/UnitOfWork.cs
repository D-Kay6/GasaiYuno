using GasaiYuno.Discord.Domain.Persistence.Repositories;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using GasaiYuno.Discord.RavenDB.Repositories;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.RavenDB;

public class UnitOfWork : IUnitOfWork
{
    private readonly IAsyncDocumentSession _session;

    private BanRepository _banRepository;
    private CommandRepository _commandRepository;
    private NotificationRepository _notificationRepository;
    private StickyMessageRepository _stickyMessageRepository;
    private DynamicChannelRepository _dynamicChannelRepository;
    private GameRoleRepository _gameRoleRepository;
    private DistributionRoleRepository _distributionRoleRepository;
    private PollRepository _pollRepository;
    private RaffleRepository _raffleRepository;
    private ServerRepository _serverRepository;

    public IBanRepository Bans => _banRepository ??= new BanRepository(_session);
    public ICommandRepository Commands => _commandRepository ??= new CommandRepository(_session);
    public INotificationRepository Notifications => _notificationRepository ??= new NotificationRepository(_session);
    public IStickyMessageRepository StickyMessages => _stickyMessageRepository ??= new StickyMessageRepository(_session);
    public IDynamicChannelRepository DynamicChannels => _dynamicChannelRepository ??= new DynamicChannelRepository(_session);
    public IGameRoleRepository GameRoles => _gameRoleRepository ??= new GameRoleRepository(_session);
    public IDistributionRoleRepository DistributionRoles => _distributionRoleRepository ??= new DistributionRoleRepository(_session);
    public IPollRepository Polls => _pollRepository ??= new PollRepository(_session);
    public IRaffleRepository Raffles => _raffleRepository ??= new RaffleRepository(_session);
    public IServerRepository Servers => _serverRepository ??= new ServerRepository(_session);

    public UnitOfWork(IDocumentStore documentStore)
    {
        _session = documentStore.OpenAsyncSession();
        _session.Advanced.UseOptimisticConcurrency = true;
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _session.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _session?.Dispose();
    }
}