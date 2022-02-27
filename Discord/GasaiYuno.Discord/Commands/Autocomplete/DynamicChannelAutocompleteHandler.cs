using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Autocomplete
{
    public class DynamicChannelAutocompleteHandler : AutocompleteHandler
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        /// <inheritdoc />
        public DynamicChannelAutocompleteHandler(Func<IUnitOfWork> unitOfWorkFactory)
        {  
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        /// <inheritdoc />
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            var unitOfWork = _unitOfWorkFactory();

            var name = autocompleteInteraction.Data?.Current?.Value?.ToString();
            var configurations = string.IsNullOrWhiteSpace(name)
                ? await unitOfWork.DynamicChannels.GetAllAsync().ConfigureAwait(false)
                : await unitOfWork.DynamicChannels.SearchAsync(context.Guild.Id, name).ConfigureAwait(false);
            
            return AutocompletionResult.FromSuccess(configurations.Take(25).Select(x => new AutocompleteResult(x.Name, x.Name)));
        }
    }
}