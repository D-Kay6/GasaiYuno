using Autofac;
using GasaiYuno.Discord.Hosting;
using GasaiYuno.Discord.Services;
using Interactivity;
using Microsoft.Extensions.Hosting;

namespace GasaiYuno.Discord.Modules
{
    internal class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DiscordService>().As<IHostedService>();
            builder.RegisterType<LifetimeService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InteractivityService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().AsSelf().InstancePerLifetimeScope();
        }
    }
}