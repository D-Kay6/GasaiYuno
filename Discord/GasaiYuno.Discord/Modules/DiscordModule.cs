﻿using Autofac;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Interactivity;
using System;

namespace GasaiYuno.Discord.Modules
{
    public class DiscordModule : Module
    {
        public string Token { get; init; }
        public int ConnectionTimeout { get; init; }
        public int HandlerTimeout { get; init; }
        public int TotalShards { get; init; }
        public string ConnectionString { get; init; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new DiscordSocketConfig
            {
#if !DEBUG
                TotalShards = TotalShards,
                LogLevel = LogSeverity.Verbose,
#else
                LogLevel = LogSeverity.Debug,
#endif
                ConnectionTimeout = ConnectionTimeout,
                HandlerTimeout = HandlerTimeout,
                GatewayIntents = GatewayIntents.Guilds |
                                 GatewayIntents.GuildMembers |
                                 GatewayIntents.GuildPresences |
                                 GatewayIntents.GuildIntegrations |
                                 GatewayIntents.GuildVoiceStates |
                                 GatewayIntents.GuildMessages |
                                 GatewayIntents.GuildMessageReactions |
                                 GatewayIntents.GuildScheduledEvents |
                                 GatewayIntents.GuildInvites |
                                 GatewayIntents.DirectMessages

            }).InstancePerDependency();
            builder.Register(x => new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            }).InstancePerDependency();
            builder.Register(x => new InteractivityConfig
            {
                DefaultTimeout = TimeSpan.FromMinutes(1)
            }).InstancePerDependency();

            builder.RegisterModule(new ConnectionModule(Token));
            builder.RegisterModule(new ListenerModule());
            builder.RegisterModule(new PersistenceModule(ConnectionString));
            builder.RegisterModule(new ServiceModule());
            builder.RegisterModule(new MediatorModule());
        }
    }
}