using Autofac;
using GasaiYuno.Interface.Listing;
using GasaiYuno.Listing.Discord.Configuration;

namespace GasaiYuno.Listing.Discord
{
    internal class ListingModule : Module
    {
        public ListingConfig ListingConfig { get; init; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UpdateService>().As<IListingUpdater>().WithParameter("listingConfig", ListingConfig).InstancePerDependency();
        }
    }
}