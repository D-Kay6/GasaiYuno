using Autofac;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Listeners;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Services;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Modules;

public class DiscordModule : Module
{
    public string Token { get; init; }
    public int ConnectionTimeout { get; init; }
    public int HandlerTimeout { get; init; }
    public int? TotalShards { get; init; }
    public LogSeverity LogLevel { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(_ => new DiscordSocketConfig
        {
            TotalShards = TotalShards,
            LogLevel = LogLevel,
            ConnectionTimeout = ConnectionTimeout,
            HandlerTimeout = HandlerTimeout,
            GatewayIntents = GatewayIntents.DirectMessages |
                             GatewayIntents.Guilds |
                             GatewayIntents.GuildBans |
                             GatewayIntents.GuildMembers |
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
        
        builder.RegisterMediatR(Assembly.GetExecutingAssembly());
        
        builder.RegisterType<DiscordService>().As<IHostedService>();
        builder.RegisterType<LifetimeService>().As<ILifetimeService>().InstancePerLifetimeScope();
        builder.RegisterType<DiscordConnectionClient>().As<DiscordShardedClient>().AsSelf().WithParameter("token", Token).InstancePerLifetimeScope();
        builder.RegisterType<InteractionService>().AsSelf().InstancePerLifetimeScope();
        builder.RegisterType<InteractiveService>().AsSelf().InstancePerLifetimeScope();
        
        builder.RegisterType<StatusListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<GuildListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<SlashCommandListener>().As<IListener>().InstancePerLifetimeScope();
    }
}