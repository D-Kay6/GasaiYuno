using Autofac;
using GasaiYuno.Interface.Storage;

namespace GasaiYuno.Storage.Image
{
    public class StorageModule : Module
    {
        public string BaseDirectory { get; init; }
        public string CoreDirectory { get; init; }
        public string[] SupportedFormats { get; init; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ImageService>().As<IImageStorage>()
                .WithParameters(new[]
                {
                    new NamedParameter("baseDirectory", BaseDirectory),
                    new NamedParameter("coreDirectory", CoreDirectory),
                    new NamedParameter("supportedFormats", SupportedFormats)
                })
                .InstancePerDependency();
        }
    }
}