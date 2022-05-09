using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.Domain.Persistence.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Autocomplete;

public class CustomCommandAutocompleteHandler : AutocompleteHandler
{
    private readonly Func<IUnitOfWork> _unitOfWorkFactory;

    /// <inheritdoc />
    public CustomCommandAutocompleteHandler(Func<IUnitOfWork> unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    /// <inheritdoc />
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var unitOfWork = _unitOfWorkFactory();

        var command = autocompleteInteraction.Data?.Current?.Value?.ToString();
        var commands = string.IsNullOrWhiteSpace(command)
            ? await unitOfWork.Commands.ListAsync(context.Guild.Id).ConfigureAwait(false)
            : await unitOfWork.Commands.SearchAsync(context.Guild.Id, command).ConfigureAwait(false);
            
        return AutocompletionResult.FromSuccess(commands.Take(25).Select(x => new AutocompleteResult(x.Command, x.Command)));
    }
}