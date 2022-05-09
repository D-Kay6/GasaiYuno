using Autofac;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using GasaiYuno.Discord.RavenDB;
using System;

namespace GasaiYuno.Discord.Modules;

public class DiscordModule : Module
{
    public string Token { get; init; }
    public int ConnectionTimeout { get; init; }
    public int HandlerTimeout { get; init; }
    public int TotalShards { get; init; }
    public string ConnectionString { get; init; }
    public string Url { get; init; }
    public string Database { get; init; }
    public string Certificate { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => new DiscordSocketConfig
        {
#if !DEBUG
            TotalShards = TotalShards,
            LogLevel = LogSeverity.Verbose,
#else
            LogLevel = LogSeverity.Debug,
#endif
            ConnectionTimeout = ConnectionTimeout,
            HandlerTimeout = HandlerTimeout,
            GatewayIntents = GatewayIntents.DirectMessages |
                             GatewayIntents.Guilds |
                             GatewayIntents.GuildBans |
                             GatewayIntents.GuildMembers |
                             GatewayIntents.GuildPresences |
                             GatewayIntents.GuildIntegrations |
                             GatewayIntents.GuildVoiceStates |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.GuildMessageReactions |
                             GatewayIntents.GuildScheduledEvents |
                             GatewayIntents.GuildInvites | GatewayIntents.GuildWebhooks

        }).InstancePerDependency();
        builder.Register(_ => new InteractionServiceConfig
        {
            DefaultRunMode = RunMode.Async,
            AutoServiceScopes = false,
            UseCompiledLambda = true,
            InteractionCustomIdDelimiters = new[] { ' ' }
        }).InstancePerDependency();
        builder.Register(_ => new InteractiveConfig
        {
            DefaultTimeout = TimeSpan.FromMinutes(1)
        }).InstancePerDependency();

        builder.RegisterModule(new ConnectionModule(Token));
        builder.RegisterModule(new ListenerModule());
        builder.RegisterModule(new PersistenceModule(Url, Database, Certificate));
        builder.RegisterModule(new ServiceModule());
        builder.RegisterModule(new MediatorModule());
    }
}