using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.DynamicChannels.Mediator.Requests;
using MediatR;

namespace GasaiYuno.Discord.DynamicChannels.Commands.Autocomplete;

public class DynamicChannelAutocompleteHandler : AutocompleteHandler
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public DynamicChannelAutocompleteHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <inheritdoc />
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter,
        IServiceProvider services)
    {
        var name = autocompleteInteraction.Data?.Current?.Value?.ToString();
        var configurations = await _mediator.Send(new SearchDynamicChannelsRequest(context.Guild.Id, name)).ConfigureAwait(false);
        return AutocompletionResult.FromSuccess(configurations.Take(25).Select(x => new AutocompleteResult(x.Name, x.Name)));
    }
}