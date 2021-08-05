using Autofac;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Handlers;
using GasaiYuno.Discord.Infrastructure;
using GasaiYuno.Discord.Infrastructure.Repositories;
using GasaiYuno.Discord.Infrastructure.UnitOfWorks;
using GasaiYuno.Discord.Persistence.Repositories;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using GasaiYuno.Discord.Services;
using Interactivity;
using Microsoft.Extensions.Logging;
using System;
using Victoria.Node;

namespace GasaiYuno.Discord.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterDiscord(this ContainerBuilder builder)
        {
            builder.RegisterType<Connection>().As<Interface.Bot.IConnection>().InstancePerLifetimeScope();

            builder.RegisterDiscordClient();
            builder.RegisterServices();
            builder.RegisterHandlers();
            builder.RegisterPersistence();

            return builder;
        }

        private static ContainerBuilder RegisterDiscordClient(this ContainerBuilder builder)
        {
            builder.Register(x => new DiscordShardedClient(new DiscordSocketConfig
            {
#if !DEBUG
                TotalShards = 5,
                LogLevel = LogSeverity.Info,
#else
                LogLevel = LogSeverity.Verbose,
#endif
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
            })).InstancePerLifetimeScope();

            builder.Register(x => new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            })).InstancePerLifetimeScope();

            builder.Register(x => new InteractivityConfig
            {
                DefaultTimeout = TimeSpan.FromMinutes(1)
            }).InstancePerLifetimeScope();

            builder.Register(x => new LavaNode(x.Resolve<DiscordShardedClient>(), new NodeConfiguration(), x.Resolve<ILogger<LavaNode>>())).InstancePerLifetimeScope();

            return builder;
        }

        private static ContainerBuilder RegisterServices(this ContainerBuilder builder)
        {
            builder.RegisterType<InteractivityService>().InstancePerLifetimeScope();
            builder.RegisterType<RestartService>().InstancePerLifetimeScope();
            builder.RegisterType<ServerService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().InstancePerLifetimeScope();

            return builder;
        }

        private static ContainerBuilder RegisterHandlers(this ContainerBuilder builder)
        {
            builder.RegisterType<StatusHandler>().As<IHandler>().InstancePerLifetimeScope();
            builder.RegisterType<GuildHandler>().As<IHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CommandHandler>().As<IHandler>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationHandler>().As<IHandler>().InstancePerLifetimeScope();
            builder.RegisterType<BanHandler>().As<IHandler>().InstancePerLifetimeScope();
            builder.RegisterType<DynamicChannelHandler>().As<IHandler>().InstancePerLifetimeScope();
            builder.RegisterType<PollHandler>().As<IHandler>().InstancePerLifetimeScope();
            builder.RegisterType<MusicHandler>().As<IHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ListingHandler>().As<IHandler>().InstancePerLifetimeScope();

            return builder;
        }

        private static ContainerBuilder RegisterPersistence(this ContainerBuilder builder)
        {
            builder.RegisterType<DataContext>().InstancePerDependency();
            
            builder.RegisterType<BanUnitOfWork>().As<IUnitOfWork<IBanRepository>>().InstancePerDependency();
            builder.RegisterType<CommandUnitOfWork>().As<IUnitOfWork<ICommandRepository>>().InstancePerDependency();
            builder.RegisterType<DynamicChannelUnitOfWork>().As<IUnitOfWork<IDynamicChannelRepository>>().InstancePerDependency();
            //builder.RegisterType<DynamicRoleUnitOfWork>().As<IUnitOfWork<IDynamicRoleRepository>>().InstancePerDependency();
            builder.RegisterType<PollUnitOfWork>().As<IUnitOfWork<IPollRepository>>().InstancePerDependency();
            builder.RegisterType<LanguageUnitOfWork>().As<IUnitOfWork<ILanguageRepository>>().InstancePerDependency();
            builder.RegisterType<NotificationUnitOfWork>().As<IUnitOfWork<INotificationRepository>>().InstancePerDependency();
            builder.RegisterType<ServerUnitOfWork>().As<IUnitOfWork<IServerRepository>>().InstancePerDependency();
            
            builder.RegisterType<BanRepository>().As<IBanRepository>().InstancePerDependency();
            builder.RegisterType<CommandRepository>().As<ICommandRepository>().InstancePerDependency();
            builder.RegisterType<DynamicChannelRepository>().As<IDynamicChannelRepository>().InstancePerDependency();
            //builder.RegisterType<DynamicRoleRepository>().As<IDynamicRoleRepository>().InstancePerDependency();
            builder.RegisterType<PollRepository>().As<IPollRepository>().InstancePerDependency();
            builder.RegisterType<LanguageRepository>().As<ILanguageRepository>().InstancePerDependency();
            builder.RegisterType<NotificationRepository>().As<INotificationRepository>().InstancePerDependency();
            builder.RegisterType<ServerRepository>().As<IServerRepository>().InstancePerDependency();

            return builder;
        }
    }
}