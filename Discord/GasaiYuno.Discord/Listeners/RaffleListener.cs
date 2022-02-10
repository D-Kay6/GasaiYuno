﻿using Discord;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Mediator.Requests;
using GasaiYuno.Discord.Models;
using GasaiYuno.Discord.Persistence.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Listeners
{
    internal class RaffleListener : IDisposable
    {
        private readonly DiscordShardedClient _client;
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;
        private readonly IMediator _mediator;
        private readonly ILogger<RaffleListener> _logger;
        private readonly Timer _timer;
        private readonly Random _random;

        public RaffleListener(DiscordConnectionClient client, Func<IUnitOfWork> unitOfWorkFactory, IMediator mediator, ILogger<RaffleListener> logger)
        {
            _client = client;
            _unitOfWorkFactory = unitOfWorkFactory;
            _mediator = mediator;
            _logger = logger;

            _timer = new Timer(CheckRaffles, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _random = new Random();

            client.Ready += OnReady;
        }

        private Task OnReady()
        {
            _client.ReactionAdded += OnReactionAdded;
            _client.ChannelDestroyed += OnChannelDestroyed;
            _client.MessageDeleted += OnMessageDeleted;

            _timer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            var channel = await cachedChannel.GetOrDownloadAsync().ConfigureAwait(false);
            if (channel is not SocketGuildChannel guildChannel) return;
            if (reaction.Emote.Name != "✅") return;

            var unitOfWork = _unitOfWorkFactory();
            var raffle = await unitOfWork.Raffles.GetAsync(guildChannel.Guild.Id, guildChannel.Id, cachedMessage.Id);
            if (raffle == null) return;
            if (!raffle.Users.Contains(reaction.UserId))
            {
                raffle.Users.Add(reaction.UserId);
                unitOfWork.Raffles.Update(raffle);
                await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            var userMessage = await cachedMessage.DownloadAsync();
            var user = reaction.User.IsSpecified ? reaction.User.Value : guildChannel.Guild.GetUser(reaction.UserId);
            await userMessage.RemoveReactionAsync(new Emoji("✅"), user).ConfigureAwait(false);
        }

        private async Task OnChannelDestroyed(SocketChannel channel)
        {
            if (channel is not SocketGuildChannel guildChannel) return;

            var unitOfWork = _unitOfWorkFactory();
            var raffles = await unitOfWork.Raffles.ListAsync(guildChannel.Guild.Id, guildChannel.Id).ConfigureAwait(false);
            if (!raffles.Any()) return;
            
            unitOfWork.Raffles.RemoveRange(raffles);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            var channel = await cachedChannel.GetOrDownloadAsync().ConfigureAwait(false);
            if (channel is not SocketGuildChannel guildChannel) return;

            var unitOfWork = _unitOfWorkFactory();
            var raffle = await unitOfWork.Raffles.GetAsync(guildChannel.Guild.Id, guildChannel.Id, cachedMessage.Id).ConfigureAwait(false);
            if (raffle == null) return;
            
            unitOfWork.Raffles.Remove(raffle);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
        }

        private async void CheckRaffles(object stateInfo)
        {
            var unitOfWork = _unitOfWorkFactory();

            var raffles = await unitOfWork.Raffles.ListAsync(expired: true).ConfigureAwait(false);
            if (raffles.Any())
            {
                foreach (var raffle in raffles)
                {
                    var guild = _client.GetGuild(raffle.Server.Id);
                    var channel = guild?.GetTextChannel(raffle.Channel);
                    if (channel == null) continue;

                    var message = await channel.GetMessageAsync(raffle.Message).ConfigureAwait(false);
                    SocketGuildUser guildUser = null;
                    for (var i = 0; i < 10; i++)
                    {
                        var randomIndex = _random.Next(0, raffle.Users.Count - 1);
                        var userWon = raffle.Users[randomIndex];
                        guildUser = guild.GetUser(userWon);
                        if (guildUser != null) break;
                    }
                    if (guildUser == null) continue;

                    var translation = await _mediator.Send(new GetTranslationRequest(raffle.Server.Language.Name)).ConfigureAwait(false);
                    var embedBuilder = new EmbedBuilder();
                    embedBuilder.WithTitle(translation.Message("Automation.Raffle.Result.Title"));
                    embedBuilder.AddField(raffle.Title, translation.Message("Automation.Raffle.Result.Winner", guildUser.Mention));
                    await channel.SendMessageAsync(embed: embedBuilder.Build(), messageReference: new MessageReference(message.Id, message.Channel.Id));
                }
                
                unitOfWork.Raffles.RemoveRange(raffles);
                await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }

            _timer.Change(TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}