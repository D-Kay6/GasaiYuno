using GasaiYuno.Discord.Domain;
using GasaiYuno.Discord.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Infrastructure.Repositories
{
    public class LanguageRepository : Repository<Language>, ILanguageRepository
    {
        /// <summary>
        /// Creates a new <see cref="LanguageRepository"/>.
        /// </summary>
        /// <param name="context">The context that will be used.</param>
        public LanguageRepository(DataContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<Language> GetAsync(string name)
        {
            return await Context.Languages
                .FirstOrDefaultAsync(x => x.Name == name || x.LocalizedName == name)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<Language>> ListAsync()
        {
            return await Context.Languages.ToListAsync().ConfigureAwait(false);
        }
    }
}