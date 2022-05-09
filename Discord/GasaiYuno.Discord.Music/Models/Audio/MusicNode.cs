using Discord.WebSocket;
using Victoria;

namespace GasaiYuno.Discord.Music.Models.Audio;

public class MusicNode : LavaNode<MusicPlayer>
{
    /// <inheritdoc />
    public MusicNode(DiscordSocketClient socketClient, LavaConfig config)
        : base(socketClient, config) { }


    /// <inheritdoc />
    public MusicNode(DiscordShardedClient shardedClient, LavaConfig config)
        : base(shardedClient, config) { }
}