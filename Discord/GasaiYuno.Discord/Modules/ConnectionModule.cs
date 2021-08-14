using Autofac;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using Victoria.Node;

namespace GasaiYuno.Discord.Modules
{
    internal class ConnectionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new DiscordShardedClient(x.Resolve<DiscordSocketConfig>())).InstancePerLifetimeScope();

            builder.RegisterType<Connection>().InstancePerLifetimeScope();
            builder.RegisterType<CommandService>().InstancePerLifetimeScope();
            builder.RegisterType<LavaNode>().InstancePerLifetimeScope();
        }
    }
}