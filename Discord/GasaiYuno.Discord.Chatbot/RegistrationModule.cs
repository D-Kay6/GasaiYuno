using Autofac;
using GasaiYuno.Discord.Chatbot.Interfaces;
using GasaiYuno.Discord.Chatbot.Listeners;
using GasaiYuno.Discord.Chatbot.Services;
using GasaiYuno.Discord.Core.Interfaces;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Chatbot;

internal class RegistrationModule : Module
{
    public string ApiKey { get; init; }
    public double IdleDuration { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        var mediatRConfig = MediatRConfigurationBuilder
            .Create(Assembly.GetExecutingAssembly())
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();
        builder.RegisterMediatR(mediatRConfig);

        builder.RegisterType<ChatListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<SessionService>().As<ISessionService>().InstancePerLifetimeScope()
            .WithParameters(new[]
            {
                new NamedParameter("apiKey", ApiKey),
                new NamedParameter("idleDuration", IdleDuration)
            });
    }
}