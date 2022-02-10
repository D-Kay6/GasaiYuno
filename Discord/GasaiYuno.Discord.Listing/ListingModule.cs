using Autofac;
using GasaiYuno.Discord.Listing.Configuration;
using GasaiYuno.Discord.Listing.Ínterfaces;
using GasaiYuno.Discord.Listing.Listeners;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Listing
{
    internal class ListingModule : Module
    {
        public ListingConfig ListingConfig { get; init; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatR(Assembly.GetExecutingAssembly());

            builder.RegisterType<ListingListener>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<UpdateService>().As<IListingUpdater>().WithParameter("listingConfig", ListingConfig).InstancePerDependency();
        }
    }
}