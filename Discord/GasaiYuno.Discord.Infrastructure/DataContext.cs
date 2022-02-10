using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure
{
    public class DataContext : DbContext
    {
        private readonly string _connectionString;

        /// <summary>Table containing all the languages.</summary>
        public DbSet<Language> Languages { get; set; }

        /// <summary>Table containing all the server.</summary>
        public DbSet<Server> Servers { get; set; }

        /// <summary>Table containing all the notifications.</summary>
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>Table containing all the bans.</summary>
        public DbSet<Ban> Bans { get; set; }

        /// <summary>Table containing all the custom commands.</summary>
        public DbSet<CustomCommand> CustomCommands { get; set; }

        /// <summary>Table containing all the dynamic channels.</summary>
        public DbSet<DynamicChannel> DynamicChannels { get; set; }

        ///// <summary>Table containing all the dynamic roles.</summary>
        //public DbSet<DynamicRole> DynamicRoles { get; set; }

        /// <summary>Table containing all the polls.</summary>
        public DbSet<Poll> Polls { get; set; }

        /// <summary>Table containing all the polls.</summary>
        public DbSet<Raffle> Raffles { get; set; }

        /// <summary>The current transaction.</summary>
        private IDbContextTransaction _currentTransaction;

        /// <summary>Whether there is an active transaction.</summary>
        /// <value><c>true</c> if there is an active transaction; otherwise, <c>false</c>.</value>
        public bool HasActiveTransaction => _currentTransaction != null;
    
        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AS");

            modelBuilder.ApplyConfiguration(new LanguageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ServerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BanEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CustomCommandEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DynamicChannelEntityTypeConfiguration());
            //modelBuilder.ApplyConfiguration(new DynamicRoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PollEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RaffleEntityTypeConfiguration());
        }

        /// <summary>
        /// Begin a new transaction on the current context.
        /// </summary>
        /// <returns>An awaitable task that returns an <see cref="IDbContextTransaction"/>.</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("A transaction is already being made.");

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);
            return _currentTransaction;
        }

        /// <summary>
        /// Commit all changes in a transaction.
        /// </summary>
        /// <param name="transaction">The transaction to commit.</param>
        /// <returns>The amount of changes that have been made.</returns>
        public int CommitTransaction(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                var result = SaveChanges();
                transaction.Commit();
                return result;
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Commit all changes in a transaction asynchronously.
        /// </summary>
        /// <param name="transaction">The transaction to commit.</param>
        /// <returns>An awaitable task that returns an <see cref="int"/>.</returns>
        public async Task<int> CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                var result = await SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
                return result;
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Revert the changes made in the current transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}