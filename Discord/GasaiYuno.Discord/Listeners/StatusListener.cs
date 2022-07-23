using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Listeners;

internal class StatusListener : IListener
{
    public int Priority => 1;

    private readonly DiscordShardedClient _client;
    private readonly Random _random;
    private readonly Timer _timer;

    public StatusListener(DiscordShardedClient client)
    {
        _client = client;
        _random = new Random();
        _timer = new Timer(RandomizeActivity, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public Task Start()
    {
        _timer.Change(TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);
        return Task.CompletedTask;
    }

    private async void RandomizeActivity(object stateInfo)
    {
#if DEBUG
        await _client.SetStatusAsync(UserStatus.DoNotDisturb).ConfigureAwait(false);
        await _client.SetGameAsync("new updates", type: ActivityType.Watching).ConfigureAwait(false);
        return;
#endif
        var i = _random.Next(1, 7);
        switch (i)
        {
            case 1:
                await _client.SetGameAsync("with her knife").ConfigureAwait(false);
                break;
            case 2:
                await _client.SetGameAsync("Yukiteru Diary", type: ActivityType.Watching).ConfigureAwait(false);
                break;
            case 3:
                await _client.SetGameAsync("the Diary Game", type: ActivityType.Competing).ConfigureAwait(false);
                break;
            case 4:
                await _client.SetGameAsync("some music", type: ActivityType.Listening).ConfigureAwait(false);
                break;
            case 5:
                await _client.SetGameAsync($"{_client.Guilds.Count} servers", type: ActivityType.Watching).ConfigureAwait(false);
                break;
            case 6:
                var userCount = _client.Guilds.Sum(guild => guild.MemberCount);
                await _client.SetGameAsync($"{userCount} users", type: ActivityType.Watching).ConfigureAwait(false);
                break;
        }
        
        _timer.Change(TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);
    }

    public ValueTask DisposeAsync()
    {
        return _timer?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}