using Autofac;
using GasaiYuno.Interface.Storage;

namespace GasaiYuno.Storage.Configuration.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterConfigStorage(this ContainerBuilder builder)
        {
            builder.RegisterType<ConfigService>().As<IConfigStorage>().InstancePerDependency();

            return builder;
        }
    }
}