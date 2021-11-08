using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Core.Resolving.Pipeline;
using GasaiYuno.Discord.Listeners;
using GasaiYuno.Discord.Services;

namespace GasaiYuno.Discord.Modules
{
    internal class ListenerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StatusListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<GuildListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<CommandListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<SlashCommandListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<NotificationListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<BanListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<DynamicChannelListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<PollListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<MusicListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<ListingListener>().AsSelf().InstancePerLifetimeScope().AutoActivate();
        }

        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
        {
            registration.PipelineBuilding += (sender, pipeline) =>
            {
                pipeline.Use(PipelinePhase.Activation, MiddlewareInsertionMode.EndOfPhase, (c, next) =>
                {
                    next(c);
                    if (c.Instance?.GetType() == typeof(LifetimeService))
                    {
                        c.Resolve<StatusListener>();
                        c.Resolve<GuildListener>();
                        c.Resolve<CommandListener>();
                        c.Resolve<SlashCommandListener>();
                        c.Resolve<NotificationListener>();
                        c.Resolve<BanListener>();
                        c.Resolve<DynamicChannelListener>();
                        c.Resolve<PollListener>();
                        c.Resolve<MusicListener>();
                        c.Resolve<ListingListener>();
                    }
                });
            };
        }
    }
}