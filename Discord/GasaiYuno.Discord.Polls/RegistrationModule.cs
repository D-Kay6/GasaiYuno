using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Polls.Listeners;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Polls;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterMediatR(Assembly.GetExecutingAssembly());
        
        builder.RegisterType<PollListener>().As<IListener>().InstancePerLifetimeScope();
    }
}