﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Models
{
    public class DiscordConnectionClient : DiscordShardedClient
    {
        public event Func<Task> Ready;
        
        private readonly string _token;
        private readonly ILogger<DiscordConnectionClient> _logger;

        private int _shardsReady = 0;

        /// <summary>
        /// Creates a new <see cref="DiscordConnectionClient"/>.
        /// </summary>
        /// <param name="config">The  <see cref="DiscordSocketConfig"/> for the connection.</param>
        /// <param name="token">The token to connect with to the discord api.</param>
        /// <param name="logger">The <see cref="ILogger{T}"/> that will log all the log messages.</param>
        public DiscordConnectionClient(DiscordSocketConfig config, string token, ILogger<DiscordConnectionClient> logger) : base(config)
        {
            _token = token;
            _logger = logger;

            ShardReady += OnShardReady;
            Log += OnLog;
        }

        /// <summary>
        /// Starts the connection to the service.
        /// </summary>
        /// <returns>A value indicating if the connection has been successfully created.</returns>
        public override async Task<bool> StartAsync()
        {
            try
            {
                await base.LoginAsync(TokenType.Bot, _token).ConfigureAwait(false);
                await base.StartAsync().ConfigureAwait(false);
                return true;
            }
            catch (HttpException e) when (e.HttpCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogError(e, "The provided discord token is invalid.");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to create a connection.");
                return false;
            }
        }

        /// <summary>
        /// Stops the connection to the service.
        /// </summary>
        public override async Task StopAsync()
        {
            await base.StopAsync().ConfigureAwait(false);
            Dispose();
        }

        private Task OnShardReady(DiscordSocketClient arg)
        {
            if (++_shardsReady == Shards.Count)
            {
                Ready?.Invoke().ConfigureAwait(false);
            }

            return Task.CompletedTask;
        }

        private Task OnLog(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(log.Exception, "{Source}: {Message}", log.Source, log.Message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(log.Exception, "{Source}: {Message}", log.Source, log.Message);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(log.Exception, "{Source}: {Message}", log.Source, log.Message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(log.Exception, "{Source}: {Message}", log.Source, log.Message);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace(log.Exception, "{Source}: {Message}", log.Source, log.Message);
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug(log.Exception, "{Source}: {Message}", log.Source, log.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return Task.CompletedTask;
        }
    }
}