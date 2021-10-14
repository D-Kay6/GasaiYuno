using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace GasaiYuno.Discord.Listeners
{
    internal class StatusListener : IDisposable
    {
        private readonly DiscordShardedClient _client;
        private readonly Random _random;
        private readonly Timer _timer;

        public StatusListener(DiscordConnectionClient client)
        {
            _client = client;
            _timer = new Timer { Interval = TimeSpan.FromMinutes(5).TotalMilliseconds };
            _random = new Random();

            client.Ready += OnReady;
        }

        private async Task OnReady()
        {
            _timer.Elapsed += OnTick;
            _timer.Start();

            await RandomizeActivityAsync().ConfigureAwait(false);
        }

        private async void OnTick(object sender, ElapsedEventArgs e)
        {
            await RandomizeActivityAsync().ConfigureAwait(false);
        }

        private async Task RandomizeActivityAsync()
        {
            var i = _random.Next(1, 6);
            IActivity activity = null;
#if DEBUG
            i = 10;
#endif
            switch (i)
            {
                case 1:
                    activity = new Game("Yukiteru Diary", ActivityType.Watching);
                    break;
                case 2:
                    activity = new Game("with her knife", ActivityType.Playing);
                    break;
                case 3:
                    activity = new Game("some music", ActivityType.Listening);
                    break;
                case 4:
                    activity = new Game($"{_client.Guilds.Count} servers", ActivityType.Watching);
                    break;
                case 5:
                    var userCount = _client.Guilds.Sum(guild => guild.MemberCount);
                    activity = new Game($"{userCount} users", ActivityType.Watching);
                    break;
                case 10:
                    await _client.SetStatusAsync(UserStatus.DoNotDisturb).ConfigureAwait(false);
                    activity = new Game("new updates", ActivityType.Watching);
                    break;
            }

            await _client.SetActivityAsync(activity).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}