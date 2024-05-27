using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Microsoft.Extensions.DependencyInjection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Core;

public abstract class RegistrationModuleBase : Module
{
    protected sealed override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        var mediatRConfig = MediatRConfigurationBuilder
            .Create(GetType().Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();
        builder.RegisterMediatR(mediatRConfig);

        RegisterComponents(builder);
        var services = new ServiceCollection();
        RegisterServices(services);
        builder.Populate(services);
    }

    protected virtual void RegisterComponents(ContainerBuilder builder) { }

    protected virtual void RegisterServices(IServiceCollection services) { }
}