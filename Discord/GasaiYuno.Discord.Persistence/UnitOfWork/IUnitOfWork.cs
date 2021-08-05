using System;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Persistence.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Begin making changes to the current data set.
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/></returns>
        Task BeginAsync();

        /// <summary>
        /// Saves all the changes in the context to the database.
        /// </summary>
        /// <returns>Returns the amount of changes it saved.</returns>
        int Save();

        /// <summary>
        /// Saves all the changes in the context to the database asynchronously.
        /// </summary>
        /// <returns>Returns the amount of changes it saved.</returns>
        Task<int> SaveAsync();
    }

    public interface IUnitOfWork<out T> : IUnitOfWork where T : class
    {
        /// <summary>
        /// The table containing all the data.
        /// </summary>
        T DataSet { get; }
    }
}