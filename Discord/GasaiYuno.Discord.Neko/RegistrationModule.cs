using Autofac;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neko_Love.Net.V1;
using Nekos.Net.V3;
using System.Reflection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Neko;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var mediatRConfig = MediatRConfigurationBuilder
            .Create(Assembly.GetExecutingAssembly())
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();
        builder.RegisterMediatR(mediatRConfig);
        
        builder.Register(x => new NekosV3Client(x.Resolve<ILogger<NekosV3Client>>())).InstancePerDependency();
        builder.Register(x => new NekoV1Client(x.Resolve<ILogger<NekoV1Client>>())).InstancePerDependency();
    }
}