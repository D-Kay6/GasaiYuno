using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Victoria.Node;

namespace GasaiYuno.Discord.Music.Models.Audio;

public class MusicNode : LavaNode<MusicPlayer, PlayableTrack>
{
    private readonly BaseSocketClient _client;
    private readonly NodeConfiguration _nodeConfiguration;

    /// <inheritdoc />
    public MusicNode(DiscordSocketClient socketClient, NodeConfiguration nodeConfiguration, ILogger<MusicNode> logger) : base(socketClient, nodeConfiguration, logger)
    {
        _client = socketClient;
        _nodeConfiguration = nodeConfiguration;
        socketClient.UserVoiceStateUpdated += OnUserVoiceStateUpdatedAsync;
    }


    /// <inheritdoc />
    public MusicNode(DiscordShardedClient shardedClient, NodeConfiguration nodeConfiguration, ILogger<MusicNode> logger) : base(shardedClient, nodeConfiguration, logger)
    {
        _client = shardedClient;
        _nodeConfiguration = nodeConfiguration;
        shardedClient.UserVoiceStateUpdated += OnUserVoiceStateUpdatedAsync;
    }
    
    public async Task MoveChannelAsync(IVoiceChannel channel)
    {
        if (!TryGetPlayer(channel.Guild, out var player))
            throw new InvalidOperationException("No player found for this guild.");
        
        if (channel.GuildId != player.VoiceChannel.GuildId)
            throw new ArgumentException("Channel must be in the same guild as the player.");
        
        if (channel.Id == player.VoiceChannel.Id)
            throw new ArgumentException("Channel must be different from the current channel.");

        await channel.ConnectAsync(_nodeConfiguration.SelfDeaf, false, true).ConfigureAwait(false);
    }

    private Task OnUserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState pastState, SocketVoiceState currentState)
    {
        if (_client.CurrentUser?.Id != user.Id)
            return Task.CompletedTask;

        if (currentState.VoiceChannel == null)
            return Task.CompletedTask;
        
        if (!TryGetPlayer(currentState.VoiceChannel.Guild, out var player))
            return Task.CompletedTask;
        
        player.SetVoiceChannel(currentState.VoiceChannel);
        return Task.CompletedTask;
    }
}