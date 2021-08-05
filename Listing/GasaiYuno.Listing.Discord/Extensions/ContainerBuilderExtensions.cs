using Autofac;
using GasaiYuno.Interface.Listing;

namespace GasaiYuno.Listing.Discord.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterListing(this ContainerBuilder builder)
        {
            builder.RegisterType<UpdateService>().As<IListingUpdater>().InstancePerDependency();

            return builder;
        }
    }
}