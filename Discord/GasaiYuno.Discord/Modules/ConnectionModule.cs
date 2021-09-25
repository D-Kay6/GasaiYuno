using Autofac;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using Victoria.Node;

namespace GasaiYuno.Discord.Modules
{
    internal class ConnectionModule : Module
    {
        private readonly string _token;

        public ConnectionModule(string token)
        {
            _token = token;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new DiscordShardedClient(x.Resolve<DiscordSocketConfig>())).InstancePerLifetimeScope();

            builder.RegisterType<Connection>().WithParameter("token", _token).InstancePerLifetimeScope();
            builder.RegisterType<CommandService>().InstancePerLifetimeScope();
            builder.RegisterType<LavaNode>().InstancePerLifetimeScope();
        }
    }
}