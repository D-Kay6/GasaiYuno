using Autofac;
using GasaiYuno.Interface.Storage;

namespace GasaiYuno.Storage.Image.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterImageStorage(this ContainerBuilder builder)
        {
            builder.RegisterType<ImageService>().As<IImageStorage>().InstancePerDependency();

            return builder;
        }
    }
}