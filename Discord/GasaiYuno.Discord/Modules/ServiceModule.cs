using Autofac;
using Fergun.Interactive;
using GasaiYuno.Discord.Hosting;
using GasaiYuno.Discord.Services;
using Microsoft.Extensions.Hosting;

namespace GasaiYuno.Discord.Modules
{
    internal class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DiscordService>().As<IHostedService>();
            builder.RegisterType<LifetimeService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InteractiveService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().AsSelf().InstancePerLifetimeScope();
        }
    }
}