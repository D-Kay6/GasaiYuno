using Autofac;
using GasaiYuno.Discord.Storage.Interfaces;
using GasaiYuno.Discord.Storage.Services;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Storage
{
    public class StorageModule : Module
    {
        public string BaseDirectory { get; init; }
        public string CoreDirectory { get; init; }
        public string[] SupportedFormats { get; init; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatR(Assembly.GetExecutingAssembly());

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