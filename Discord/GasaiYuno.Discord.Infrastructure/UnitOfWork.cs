using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        private BanRepository _banRepository;
        private CommandRepository _commandRepository;
        private DynamicChannelRepository _dynamicChannelRepository;
        private DynamicRoleRepository _dynamicRoleRepository;
        private LanguageRepository _languageRepository;
        private NotificationRepository _notificationRepository;
        private PollRepository _pollRepository;
        private ServerRepository _serverRepository;

        public IBanRepository Bans => _banRepository ??= new BanRepository(_context);
        public ICommandRepository Commands => _commandRepository ??= new CommandRepository(_context);
        public IDynamicChannelRepository DynamicChannels => _dynamicChannelRepository ??= new DynamicChannelRepository(_context);
        public IDynamicRoleRepository DynamicRoles => _dynamicRoleRepository ??= new DynamicRoleRepository(_context);
        public ILanguageRepository Languages => _languageRepository ??= new LanguageRepository(_context);
        public INotificationRepository Notifications => _notificationRepository ??= new NotificationRepository(_context);
        public IPollRepository Polls => _pollRepository ??= new PollRepository(_context);
        public IServerRepository Servers => _serverRepository ??= new ServerRepository(_context);

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}