using Autofac;
using GasaiYuno.Discord.Services;
using Interactivity;

namespace GasaiYuno.Discord.Modules
{
    internal class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LifetimeService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InteractivityService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ServerService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().AsSelf().InstancePerLifetimeScope();
        }
    }
}