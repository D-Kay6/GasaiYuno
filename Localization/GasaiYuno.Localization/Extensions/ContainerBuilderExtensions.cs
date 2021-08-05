using Autofac;
using GasaiYuno.Interface.Localization;

namespace GasaiYuno.Localization.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterLocalization(this ContainerBuilder builder)
        {
            builder.RegisterType<LocalizationService>().As<ILocalization>().InstancePerLifetimeScope();

            return builder;
        }
    }
}