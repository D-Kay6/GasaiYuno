using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.StickyMessages.Listeners;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.StickyMessages;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterMediatR(Assembly.GetExecutingAssembly());
        
        builder.RegisterType<StickyMessageListener>().As<IListener>().InstancePerLifetimeScope();
    }
}