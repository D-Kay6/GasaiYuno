using System.Text.RegularExpressions;
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
        _interactionService.LocalizationManager = new JsonLocalizationManager("Files", "Translation");
        try
        {
            await _mediator.Publish(new RegisterCommandsEvent()).ConfigureAwait(false);
#if DEBUG
            var guild = _client.GetGuild(571289745049780224);
            await _interactionService.AddModulesToGuildAsync(guild, true, _interactionService.Modules.ToArray()).ConfigureAwait(false);
            guild = _client.GetGuild(897984723241009172);
            await _interactionService.AddModulesToGuildAsync(guild, true, _interactionService.Modules.ToArray()).ConfigureAwait(false);
            guild = _client.GetGuild(1005806268751032360);
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

    private void WriteTranslations(IEnumerable<ModuleInfo> modules)
    {
        var translations = new Dictionary<string, TranslationValue>();
        foreach (var moduleInfo in modules)
        {
            if (moduleInfo.Parent != null)
                continue;

            if (!moduleInfo.IsTopLevelGroup && !moduleInfo.IsSlashGroup)
            {
                foreach (var slashCommand in moduleInfo.SlashCommands)
                    WriteTranslations(slashCommand, ref translations);
                
                continue;
            }

            WriteTranslations(moduleInfo, ref translations);
        }

        var json = translations.ToJson();
        File.WriteAllText("Files/Translation.en-GB.json", json);
    }

    private void WriteTranslations(ModuleInfo moduleInfo, ref Dictionary<string, TranslationValue> translations)
    {
        if (moduleInfo.SlashCommands.Count == 0 && moduleInfo.SubModules.Count == 0)
            return;

        var moduleTranslations = new Dictionary<string, TranslationValue>();
        moduleTranslations.Add("name", moduleInfo.IsSlashGroup ? moduleInfo.SlashGroupName : moduleInfo.Name);
        moduleTranslations.Add("description", moduleInfo.Description);

        foreach (var slashCommand in moduleInfo.SlashCommands)
            WriteTranslations(slashCommand, ref moduleTranslations);

        foreach (var subModule in moduleInfo.SubModules)
            WriteTranslations(subModule, ref moduleTranslations);

        translations.Add(moduleInfo.IsSlashGroup ? moduleInfo.SlashGroupName : moduleInfo.Name, moduleTranslations);
    }

    private void WriteTranslations(SlashCommandInfo slashCommand, ref Dictionary<string, TranslationValue> translations)
    {
        var commandTranslations = new Dictionary<string, TranslationValue>();
        commandTranslations.Add("name", slashCommand.Name);
        commandTranslations.Add("description", slashCommand.Description);
        foreach (var parameter in slashCommand.Parameters)
        {
            var parameterTranslations = new Dictionary<string, TranslationValue>();
            parameterTranslations.Add("name", parameter.Name);
            parameterTranslations.Add("description", parameter.Description);
            var parameterNames = parameter.ParameterType.IsEnum ? Enum.GetNames(parameter.ParameterType) : parameter.Choices.Select(x => x.Name).ToArray();
            foreach (var parameterName in parameterNames)
            {
                var choiceTranslations = new Dictionary<string, TranslationValue>();
                choiceTranslations.Add("name", parameterName);
                parameterTranslations.Add(parameterName, choiceTranslations);
            }

            commandTranslations.Add(parameter.Name, parameterTranslations);
        }

        translations.Add(slashCommand.Name, commandTranslations);
    }

    private async Task ClientOnMessageReceived(SocketMessage message)
    {
        try
        {
            if (message.Author.IsBot)
                return;
            if (message is not SocketUserMessage { Channel: SocketGuildChannel channel } msg)
                return;

            var server = await _mediator.Send(new GetServerRequest(channel.Guild.Id, channel.Guild.Name)).ConfigureAwait(false);
            if (server.WarningDisabled)
                return;
            if (!msg.Content.StartsWith(server.Prefix) && !msg.Content.StartsWith(_client.CurrentUser.Mention))
                return;

            _logger.LogInformation("{@Server}: {@User} tried to run command '{Command}'", new { channel.Guild.Name, channel.Guild.Id }, new { message.Author.Username, message.Author.Id },
                message.Content);
            await message.Channel
                .SendMessageAsync(
                    "I no longer work with custom prefixes. To use my commands, start with / and either select it from the menu that appears or type out the command. If this is not working for you, join my support discord https://discord.gg/YFqUMDT and let us know. An admin can disable this message with `/disable-warning`")
                .ConfigureAwait(false);
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
            if (interaction.User.IsBot)
                return;
            if (interaction.IsDMInteraction)
                return;
#if DEBUG
            if (!interaction.User.Id.Equals(255453041531158538) && !interaction.User.Id.Equals(230619285507014657))
            {
                await interaction.RespondAsync("Sorry, I cannot do that right now. I'm under development").ConfigureAwait(false);
                return;
            }
#endif
            var context = new ShardedInteractionContext<T>(_client, interaction);
            _logger.LogInformation("{@Server}: {@User} performed interaction '{@Interaction}'", new { context.Guild.Name, context.Guild.Id }, new { context.User.Username, context.User.Id },
                new { interaction.Id, interaction.Data });
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception occurred during an interaction {@Interaction}", interaction);
        }
    }

    private async Task OnSlashCommandExecuted(SlashCommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess)
            return;

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
        if (result.IsSuccess)
            return Task.CompletedTask;

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
        if (result.IsSuccess)
            return Task.CompletedTask;

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