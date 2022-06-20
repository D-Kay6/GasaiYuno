using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Notifications.Listeners;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Notifications;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterMediatR(Assembly.GetExecutingAssembly());
        
        builder.RegisterType<NotificationListener>().As<IListener>().InstancePerLifetimeScope();
    }
}