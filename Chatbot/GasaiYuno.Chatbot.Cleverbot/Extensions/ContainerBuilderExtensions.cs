using Autofac;
using GasaiYuno.Chatbot.Cleverbot.Models;
using GasaiYuno.Interface.Chatbot;

namespace GasaiYuno.Chatbot.Cleverbot.Extensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterCleverbot(this ContainerBuilder builder)
        {
            builder.RegisterType<SessionService>().As<IChatService>().InstancePerLifetimeScope();

            return builder;
        }
    }
}