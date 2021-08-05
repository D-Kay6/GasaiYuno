using GasaiYuno.Discord.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Persistence.Repositories
{
    public interface ILanguageRepository : IRepository<Language>
    {
        /// <summary>
        /// Gets the language object asynchronously.
        /// </summary>
        /// <param name="name">The name of the language.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="Language"/>.</returns>
        Task<Language> GetAsync(string name);

        /// <summary>
        /// Gets all the languages from the database.
        /// </summary>
        /// <returns>An awaitable <see cref="List{T}"/> that returns a <see cref="Language"/>.</returns>
        Task<List<Language>> ListAsync();
    }
}