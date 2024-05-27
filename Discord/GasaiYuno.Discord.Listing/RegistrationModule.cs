using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Listing.Configuration;
using GasaiYuno.Discord.Listing.Interfaces;
using GasaiYuno.Discord.Listing.Listeners;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Listing;

internal class RegistrationModule : Module
{
    public ListingConfig ListingConfig { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        var mediatRConfig = MediatRConfigurationBuilder
            .Create(Assembly.GetExecutingAssembly())
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();
        builder.RegisterMediatR(mediatRConfig);

        builder.RegisterType<ListingListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<UpdateService>().As<IListingUpdater>().WithParameter("listingConfig", ListingConfig).InstancePerDependency();
    }
}