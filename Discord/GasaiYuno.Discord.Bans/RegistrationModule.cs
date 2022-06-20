using Autofac;
using GasaiYuno.Discord.Bans.Listeners;
using GasaiYuno.Discord.Core.Interfaces;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Bans;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterMediatR(Assembly.GetExecutingAssembly());
        
        builder.RegisterType<BanListener>().As<IListener>().InstancePerLifetimeScope();
    }
}