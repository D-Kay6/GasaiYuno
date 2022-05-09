using Autofac;
using GasaiYuno.Discord.Core.Interfaces;
using GasaiYuno.Discord.Music.Interfaces.Lyrics;
using GasaiYuno.Discord.Music.Listeners;
using GasaiYuno.Discord.Music.Models.Audio;
using GasaiYuno.Discord.Music.Services;
using MediatR.Extensions.Autofac.DependencyInjection;
using System.Reflection;
using Victoria;
using Module = Autofac.Module;

namespace GasaiYuno.Discord.Music;

internal class MusicModule : Module
{
    public string Token { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterMediatR(Assembly.GetExecutingAssembly());

        builder.Register(_ => new LavaConfig()).InstancePerDependency();
        builder.RegisterType<MusicNode>().InstancePerLifetimeScope();

        builder.RegisterType<MusicListener>().As<IListener>().InstancePerLifetimeScope();

        builder.RegisterType<LyricsService>().As<ILyricsService>().WithParameter("token", Token).InstancePerDependency();
    }
}