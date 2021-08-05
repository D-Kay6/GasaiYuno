using Discord;
using Discord.Net;
using Discord.WebSocket;
using GasaiYuno.Discord.Handlers;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Services;
using GasaiYuno.Interface.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GasaiYuno.Discord
{
    public class Connection : Interface.Bot.IConnection
    {
        private readonly DiscordShardedClient _client;
        private readonly RestartService _restartService;
        private readonly IEnumerable<IHandler> _handlers;
        private readonly IConfigStorage _configStorage;
        private readonly ILogger<Connection> _logger;

        private int _shardsReady = 0;

        /// <inheritdoc />
        public bool Restart { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Connection"/>.
        /// </summary>
        /// <param name="client">The <see cref="DiscordShardedClient"/> that will be used.</param>
        /// <param name="restartService">The <see cref="RestartService"/> that will be used for restarts.</param>
        /// <param name="handlers">The <see cref="IEnumerable{T}"/> containing all <see cref="IHandler"/>.</param>
        /// <param name="configStorage">The <see cref="IConfigStorage"/> that will be used for reading the configuration file.</param>
        /// <param name="logger">The <see cref="ILogger{T}"/> that will log all the log messages.</param>
        public Connection(DiscordShardedClient client, RestartService restartService, IEnumerable<IHandler> handlers, IConfigStorage configStorage, ILogger<Connection> logger)
        {
            _client = client;
            _restartService = restartService;
            _handlers = handlers;
            _configStorage = configStorage;
            _logger = logger;

            _client.ShardReady += OnShardReady;
            _client.Log += OnLog;
        }

        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            try
            {
                var config = _configStorage.Read<Configuration.DiscordConfig>();
                await _client.LoginAsync(TokenType.Bot, config.Token).ConfigureAwait(false);
                await _client.StartAsync().ConfigureAwait(false);
                await _restartService.AwaitRestart().ConfigureAwait(false);
                Restart = _restartService.KeepAlive;
            }
            catch (HttpException e) when (e.HttpCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogError(e, "The provided discord token is invalid.");
                Restart = false;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Fatal error occurred. Trying to restart...");
                await _client.StopAsync().ConfigureAwait(false);
            }
            finally
            {
                _client.Dispose();
            }
        }

        /// <inheritdoc />
        public Task DisconnectAsync()
        {
            _restartService.Shutdown();
            return Task.CompletedTask;
        }

        private async Task OnShardReady(DiscordSocketClient arg)
        {
            if (++_shardsReady != _client.Shards.Count) return;
            foreach (var handler in _handlers)
            {
                try
                {
                    await handler.Ready().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "The ready event for a handler {Handler} failed unexpectedly.", handler);
                    throw;
                }
            }
        }

        private Task OnLog(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}