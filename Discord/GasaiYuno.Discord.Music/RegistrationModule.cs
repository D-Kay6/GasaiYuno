using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Music.Interfaces.Lyrics;
using GasaiYuno.Discord.Music.Listeners;
using GasaiYuno.Discord.Music.Services;
using GasaiYuno.Discord.Core;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Logging;
using Lavalink4NET.Logging.Microsoft;
using Lavalink4NET.MemoryCache;

namespace GasaiYuno.Discord.Music;

internal class RegistrationModule : RegistrationModuleBase
{
    public string Token { get; init; }

    protected override void RegisterComponents(ContainerBuilder builder)
    {
        builder.Register(_ => new LavalinkNodeOptions
        {
            RestUri = "http://localhost:2333",
            WebSocketUri = "ws://localhost:2333",
            DisconnectOnStop = true
        }).InstancePerDependency();

        builder.RegisterType<MicrosoftExtensionsLogger>().As<ILogger>().InstancePerLifetimeScope();
        builder.RegisterType<LavalinkCache>().As<ILavalinkCache>().InstancePerLifetimeScope();
        builder.RegisterType<LavalinkNode>().As<IAudioService>().InstancePerLifetimeScope();
        builder.RegisterType<DiscordClientWrapper>().As<IDiscordClientWrapper>().InstancePerLifetimeScope();
        builder.RegisterType<MusicListener>().As<IListener>().InstancePerLifetimeScope();
        builder.RegisterType<LyricsService>().As<ILyricsService>().WithParameter("token", Token).InstancePerDependency();
    }
}