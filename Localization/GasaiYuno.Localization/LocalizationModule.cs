using Autofac;
using GasaiYuno.Interface.Localization;

namespace GasaiYuno.Localization
{
    internal class LocalizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LocalizationService>().As<ILocalization>().InstancePerLifetimeScope();
        }
    }
}