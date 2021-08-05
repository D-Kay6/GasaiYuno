using GasaiYuno.Discord.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private IDbContextTransaction _transaction;

        /// <summary>
        /// Creates a new <see cref="UnitOfWork"/>
        /// </summary>
        /// <param name="context">The DbContext that will be used.</param>
        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task BeginAsync()
        {
            _transaction = await _context.BeginTransactionAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public int Save()
        {
            return _context.CommitTransaction(_transaction);
        }

        /// <inheritdoc/>
        public Task<int> SaveAsync()
        {
            return _context.CommitTransactionAsync(_transaction);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}