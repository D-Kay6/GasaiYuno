using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.ExtNamePP.Mediator.Requests;
using MediatR;

namespace GasaiYuno.Discord.ExtNamePP.Commands.Autocomplete;

public class ExtNamePSAutocompleteHandler : AutocompleteHandler
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public ExtNamePSAutocompleteHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <inheritdoc />
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var input = autocompleteInteraction.Data?.Current?.Value?.ToString();
        var ExtNameCP = await _mediator.Send(new SearchExtNamePPRequest(context.Guild.Id, input)).ConfigureAwait(false);
        return AutocompletionResult.FromSuccess(ExtNameCP.Take(25).Select(x => new AutocompleteResult(, )));
    }
}