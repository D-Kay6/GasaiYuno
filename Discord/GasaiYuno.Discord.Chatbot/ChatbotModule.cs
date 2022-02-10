using Autofac;
using GasaiYuno.Discord.Chatbot.Interfaces;
using GasaiYuno.Discord.Chatbot.Services;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Chatbot
{
    internal class ChatbotModule : Module
    {
        public string ApiKey { get; init; }
        public double IdleDuration { get; init; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMediatR(Assembly.GetExecutingAssembly());
            builder.RegisterType<SessionService>().As<IChatService>()
                .WithParameters(new[]
                {
                    new NamedParameter("apiKey", ApiKey),
                    new NamedParameter("idleDuration", IdleDuration)
                })
                .InstancePerLifetimeScope();
        }
    }
}