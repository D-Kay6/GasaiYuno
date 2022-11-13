using Discord;
using Discord.Interactions;
using GasaiYuno.Discord.AutoChannels.Mediator.Requests;
using MediatR;

namespace GasaiYuno.Discord.AutoChannels.Commands.Autocomplete;

public class AutoChannelAutocompleteHandler : AutocompleteHandler
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public AutoChannelAutocompleteHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <inheritdoc />
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var input = autocompleteInteraction.Data?.Current?.Value?.ToString();
        var results = new List<AutocompleteResult>();
        var autoChannels = await _mediator.Send(new ListAutoChannelsRequest(context.Guild.Id)).ConfigureAwait(false);
        foreach (var autoChannel in autoChannels)
        {
            if (results.Count == 25) break;
            
            var voiceChannel = await context.Guild.GetVoiceChannelAsync(autoChannel.Channel).ConfigureAwait(false);
            if (voiceChannel == null) continue;
            if (!string.IsNullOrWhiteSpace(input) && !voiceChannel.Name.Contains(input, StringComparison.OrdinalIgnoreCase)) continue;
            
            results.Add(new AutocompleteResult(voiceChannel.Name, autoChannel));
        }
        
        return AutocompletionResult.FromSuccess(results);
    }
}