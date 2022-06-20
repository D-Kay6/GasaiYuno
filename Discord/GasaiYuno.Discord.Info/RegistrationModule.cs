using Autofac;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Info;

public class RegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterMediatR(Assembly.GetExecutingAssembly());
    }
}