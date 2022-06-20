using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.CustomCommands.Mediator.Requests;
using MediatR;

namespace GasaiYuno.Discord.CustomCommands.Commands.Autocomplete;

public class CustomCommandAutocompleteHandler : AutocompleteHandler
{
    private readonly IMediator _mediator;
    
    public CustomCommandAutocompleteHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <inheritdoc />
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var command = autocompleteInteraction.Data?.Current?.Value?.ToString();
        var commands = await _mediator.Send(new SearchCustomCommandsRequest(context.Guild.Id, command));
        return AutocompletionResult.FromSuccess(commands.Take(25).Select(x => new AutocompleteResult(x.Command, x.Command)));
    }
}