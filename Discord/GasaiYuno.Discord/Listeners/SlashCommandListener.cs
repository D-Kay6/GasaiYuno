using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands.TypeConverters;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Core.Mediator.Events;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Core.Models;
using GasaiYuno.Discord.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners;

public class SlashCommandListener : IListener
{
    public int Priority => 100;

    private readonly DiscordShardedClient _client;
    private readonly InteractionService _interactionService;
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SlashCommandListener> _logger;

    public SlashCommandListener(DiscordConnectionClient client, InteractionService interactionService, IMediator mediator, IServiceProvider serviceProvider, ILogger<SlashCommandListener> logger)
    {
        _client = client;
        _interactionService = interactionService;
        _mediator = mediator;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Start()
    {
        _client.MessageReceived += ClientOnMessageReceived;

        _client.AutocompleteExecuted += OnInteraction;
        _client.SlashCommandExecuted += OnInteraction;
        _client.MessageCommandExecuted += OnInteraction;
        _client.UserCommandExecuted += OnInteraction;
        _client.SelectMenuExecuted += OnInteraction;
        _client.ButtonExecuted += OnInteraction;
        _client.ModalSubmitted += OnInteraction;

        _interactionService.SlashCommandExecuted += OnSlashCommandExecuted;
        _interactionService.ComponentCommandExecuted += OnComponentCommandExecuted;
        _interactionService.ModalCommandExecuted += OnModalCommandExecuted;
        _interactionService.Log += OnLogAsync;

        _interactionService.AddTypeConverter<IMessage>(new MessageTypeConverter());
        _interactionService.AddTypeConverter<Languages>(new LanguagesTypeConverter());
        try
        {
            await _mediator.Publish(new RegisterCommandsEvent()).ConfigureAwait(false);
#if DEBUG
            var guild = _client.GetGuild(571289745049780224);
            await _interactionService.AddModulesToGuildAsync(guild, true, _interactionService.Modules.ToArray()).ConfigureAwait(false);
            guild = _client.GetGuild(897984723241009172);
            await _interactionService.AddModulesToGuildAsync(guild, true, _interactionService.Modules.ToArray()).ConfigureAwait(false);
#else
            await _interactionService.RegisterCommandsGloballyAsync().ConfigureAwait(false);

            var privateModules = _interactionService.Modules.Where(x => x.DontAutoRegister).ToArray();
            var guild = _client.GetGuild(571289745049780224);
            await _interactionService.AddModulesToGuildAsync(guild, true, privateModules).ConfigureAwait(false);

            guild = _client.GetGuild(274638156966526977);
            await _interactionService.AddModulesToGuildAsync(guild, true, privateModules).ConfigureAwait(false);
#endif
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to register commands");
        }
    }

    private async Task ClientOnMessageReceived(SocketMessage message)
    {
        try
        {
            if (message.Author.IsBot) return;
            if (message is not SocketUserMessage { Channel: SocketGuildChannel channel } msg) return;

            var server = await _mediator.Send(new GetServerRequest(channel.Guild.Id, channel.Guild.Name)).ConfigureAwait(false);
            if (server.WarningDisabled) return;
            if (!msg.Content.StartsWith(server.Prefix) && !msg.Content.StartsWith(_client.CurrentUser.Mention)) return;

            _logger.LogInformation("{@Server}: {@User} tried to run command '{Command}'", new { channel.Guild.Name, channel.Guild.Id }, new { message.Author.Username, message.Author.Id }, message.Content);
            await message.Channel.SendMessageAsync("I no longer work with custom prefixes. To use my commands, start with / and either select it from the menu that appears or type out the command. If this is not working for you, join my support discord https://discord.gg/YFqUMDT and let us know. An admin can disable this message with `/disable-warning`").ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception occurred in the command listener");
        }
    }

    private async Task OnInteraction<T>(T interaction) where T : SocketInteraction
    {
        try
        {
            if (interaction.User.IsBot) return;
            if (interaction.IsDMInteraction) return;
#if DEBUG
            if (!interaction.User.Id.Equals(255453041531158538) && !interaction.User.Id.Equals(230619285507014657))
            {
                await interaction.RespondAsync("Sorry, I cannot do that right now. I'm under development").ConfigureAwait(false);
                return;
            }
#endif
            var context = new ShardedInteractionContext<T>(_client, interaction);
            _logger.LogInformation("{@Server}: {@User} performed interaction '{@Interaction}'", new { context.Guild.Name, context.Guild.Id }, new { context.User.Username, context.User.Id }, new { interaction.Id, interaction.Data });
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception occurred during an interaction {@Interaction}", interaction);
        }
    }

    private async Task OnSlashCommandExecuted(SlashCommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess) return;

        _logger.LogWarning("Execution of {@Command} failed with {Error} {Reason}", commandInfo, result.Error, result.ErrorReason);

        var server = await _mediator.Send(new GetServerRequest(context.Guild.Id, context.Guild.Name)).ConfigureAwait(false);
        var translation = await _mediator.Send(new GetTranslationRequest(server.Language)).ConfigureAwait(false);
        var message = translation.Message("Generic.Invalid.Command");
        if (!string.IsNullOrEmpty(result.ErrorReason)) 
            message = translation.Message("Generic.Invalid.Execution", result.ErrorReason);

        await context.Interaction.RespondAsync(message).ConfigureAwait(false);
    }

    private Task OnComponentCommandExecuted(ComponentCommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess) return Task.CompletedTask;

        _logger.LogWarning("Execution of {@Command} failed with {Error} {Reason}", commandInfo, result.Error, result.ErrorReason);
        return Task.CompletedTask;

        // var server = await _mediator.Send(new GetServerRequest(context.Guild.Id, context.Guild.Name)).ConfigureAwait(false);
        // var translation = await _mediator.Send(new GetTranslationRequest(server.Language)).ConfigureAwait(false);
        // var message = translation.Message("Generic.Invalid.Command");
        // if (!string.IsNullOrEmpty(result.ErrorReason)) 
        //     message = translation.Message("Generic.Invalid.Execution", result.ErrorReason);
        //
        // await context.Interaction.RespondAsync(message).ConfigureAwait(false);
    }

    private Task OnModalCommandExecuted(ModalCommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess) return Task.CompletedTask;

        _logger.LogWarning("Execution of {@Command} failed with {Error} {Reason}", commandInfo, result.Error, result.ErrorReason);
        return Task.CompletedTask;

        // var server = await _mediator.Send(new GetServerRequest(context.Guild.Id, context.Guild.Name)).ConfigureAwait(false);
        // var translation = await _mediator.Send(new GetTranslationRequest(server.Language)).ConfigureAwait(false);
        // var message = translation.Message("Generic.Invalid.Command");
        // if (!string.IsNullOrEmpty(result.ErrorReason)) 
        //     message = translation.Message("Generic.Invalid.Execution", result.ErrorReason);
        //
        // await context.Interaction.RespondAsync(message).ConfigureAwait(false);
    }

    private Task OnLogAsync(LogMessage logMessage)
    {
        _logger.LogError(logMessage.Exception, "Unhandled exception occurred during the handling of a slash command {Source}", logMessage.Source);
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _interactionService?.Dispose();
        return ValueTask.CompletedTask;
    }
}