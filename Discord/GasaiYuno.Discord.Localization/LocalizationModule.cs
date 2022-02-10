using Autofac;
using GasaiYuno.Discord.Localization.Interfaces;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Localization
{
    internal class LocalizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatR(Assembly.GetExecutingAssembly());

            builder.RegisterType<LocalizationService>().As<ILocalization>().InstancePerLifetimeScope();
        }
    }
}