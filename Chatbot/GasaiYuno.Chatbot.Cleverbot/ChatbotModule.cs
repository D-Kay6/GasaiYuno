using Autofac;
using GasaiYuno.Interface.Chatbot;

namespace GasaiYuno.Chatbot.Cleverbot
{
    internal class ChatbotModule : Module
    {
        public string ApiKey { get; init; }
        public double IdleDuration { get; init; }

        protected override void Load(ContainerBuilder builder)
        {
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