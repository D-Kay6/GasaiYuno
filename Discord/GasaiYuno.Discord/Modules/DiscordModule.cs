using Autofac;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Services;
using Interactivity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Victoria.Node;

namespace GasaiYuno.Discord.Modules
{
    public class DiscordModule : Module
    {
        private readonly IConfiguration _configuration;

        public DiscordModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new DiscordSocketConfig
            {
#if !DEBUG
                TotalShards = 5,
#endif
                LogLevel = LogSeverity.Verbose,
                ConnectionTimeout = 60000,
                HandlerTimeout = 6000,
                GatewayIntents = GatewayIntents.Guilds |
                                 GatewayIntents.GuildMembers |
                                 GatewayIntents.GuildPresences |
                                 GatewayIntents.GuildIntegrations |
                                 GatewayIntents.GuildVoiceStates |
                                 GatewayIntents.GuildMessages |
                                 GatewayIntents.GuildMessageReactions |
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
            builder.Register(x => new NodeConfiguration()).InstancePerDependency();

            builder.RegisterModule(new ConnectionModule());
            builder.RegisterModule(new ListenerModule());
            builder.RegisterModule(new PersistenceModule());
            builder.RegisterModule(new ServiceModule());

            builder.RegisterType<DiscordService>().As<IHostedService>();
        }
    }
}