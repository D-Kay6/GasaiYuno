using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Raffles.Listeners;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Raffles;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterMediatR(Assembly.GetExecutingAssembly());
        
        builder.RegisterType<RaffleListener>().As<IListener>().InstancePerLifetimeScope();
    }
}