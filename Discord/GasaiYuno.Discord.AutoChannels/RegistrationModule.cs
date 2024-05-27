using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.AutoChannels.Listeners;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.AutoChannels;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var mediatRConfig = MediatRConfigurationBuilder
            .Create(Assembly.GetExecutingAssembly())
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();
        builder.RegisterMediatR(mediatRConfig);

        builder.RegisterType<AutoChannelListener>().As<IListener>().InstancePerLifetimeScope();
    }
}