using Autofac;
using GasaiYuno.Discord.Music.Services;
using GasaiYuno.Interface.Music;

namespace GasaiYuno.Discord.Music
{
    internal class MusicModule : Module
    {
        public string Token { get; init; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LyricsService>().As<ILyricsService>().WithParameter("token", Token).InstancePerDependency();
        }
    }
}