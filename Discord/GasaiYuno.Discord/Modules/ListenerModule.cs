using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Listeners;
using GasaiYuno.Discord.Services;

namespace GasaiYuno.Discord.Modules;

internal class ListenerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<StatusListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<GuildListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<SlashCommandListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<NotificationListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<BanListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<DynamicChannelListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<DistributionRoleListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<PollListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<RaffleListener>().As<IListener>().InstancePerLifetimeScope();
    }

    // protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
    // {
    //     registration.PipelineBuilding += (_, pipeline) =>
    //     {
    //         pipeline.Use(PipelinePhase.Activation, MiddlewareInsertionMode.EndOfPhase, (c, next) =>
    //         {
    //             next(c);
    //             if (c.Instance?.GetType() == typeof(LifetimeService))
    //             {
    //                 // c.Resolve<StatusListener>();
    //                 // c.Resolve<GuildListener>();
    //                 c.Resolve<SlashCommandListener>();
    //                 // c.Resolve<NotificationListener>();
    //                 // c.Resolve<BanListener>();
    //                 // c.Resolve<DynamicChannelListener>();
    //                 // c.Resolve<DistributionRoleListener>();
    //                 // c.Resolve<PollListener>();
    //                 // c.Resolve<RaffleListener>();
    //             }
    //         });
    //     };
    // }
}