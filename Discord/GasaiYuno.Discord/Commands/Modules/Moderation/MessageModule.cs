using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GasaiYuno.Discord.Core.Commands.Modules;
using GasaiYuno.Discord.Core.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Commands.Modules.Moderation
{
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [Group("Message")]
    public class MessageModule : BaseModule<MessageModule>
    {
        [Group("Move")]
        public class MessageMoveModule : BaseModule<MessageModule>
        {
            private readonly string[] _imageExtensions = { "jpg", "jpeg", "png", "gif" };

            [Command]
            public async Task MessageMoveDefaultAsync(SocketTextChannel channel, bool deleteOriginal = true)
            {
                await Context.Message.DeleteAsync().ConfigureAwait(false);

                var message = await GetMessageAsync().ConfigureAwait(false);
                if (message == null)
                {
                    await ReplyAsync("I could not find the message. Are you sure there are any?");
                    return;
                }

                await MoveMessageEmbedAsync(message, channel).ConfigureAwait(false);

                if (deleteOriginal)
                    await message.DeleteAsync().ConfigureAwait(false);
            }

            [Command]
            public async Task MessageMoveDefaultAsync(SocketTextChannel channel, ulong messageId, bool deleteOriginal = true)
            {
                await Context.Message.DeleteAsync().ConfigureAwait(false);

                var message = await GetMessageAsync(messageId).ConfigureAwait(false);
                if (message == null)
                {
                    await ReplyAsync("I could not find the message. Are you sure you have the right ID?").ConfigureAwait(false);
                    return;
                }

                await MoveMessageEmbedAsync(message, channel).ConfigureAwait(false);

                if (deleteOriginal)
                    await message.DeleteAsync().ConfigureAwait(false);
            }

            [Command("Plain")]
            public async Task MessageMovePlainAsync(SocketTextChannel channel, bool deleteOriginal = true)
            {
                await Context.Message.DeleteAsync().ConfigureAwait(false);

                var message = await GetMessageAsync().ConfigureAwait(false);
                if (message == null)
                {
                    await ReplyAsync("I could not find the message. Are you sure there are any?").ConfigureAwait(false);
                    return;
                }

                await MoveMessageTextAsync(message, channel).ConfigureAwait(false);

                if (deleteOriginal)
                    await message.DeleteAsync().ConfigureAwait(false);
            }

            [Command("Plain")]
            public async Task MessageMovePlainAsync(SocketTextChannel channel, ulong messageId, bool deleteOriginal = true)
            {
                await Context.Message.DeleteAsync().ConfigureAwait(false);

                var message = await GetMessageAsync(messageId).ConfigureAwait(false);
                if (message == null)
                {
                    await ReplyAsync("I could not find the message. Are you sure you have the right ID?").ConfigureAwait(false);
                    return;
                }

                await MoveMessageTextAsync(message, channel).ConfigureAwait(false);

                if (deleteOriginal)
                    await message.DeleteAsync().ConfigureAwait(false);
            }

            private async Task<IMessage> GetMessageAsync(ulong messageId = 0)
            {
                if (messageId > 0) 
                    return await Context.Channel.GetMessageAsync(messageId).ConfigureAwait(false);

                var result = Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 1);
                var messages = await result.Skip(1).FirstOrDefaultAsync().ConfigureAwait(false);
                return messages.FirstOrDefault();
            }

            private async Task MoveMessageEmbedAsync(IMessage message, SocketTextChannel channel)
            {
                var embedBuilder = new EmbedBuilder();
                embedBuilder.WithThumbnailUrl(message.Author.GetAvatarUrl());
                embedBuilder.WithAuthor((message.Author as IGuildUser)?.Nickname() ?? message.Author.Username);
                embedBuilder.WithTimestamp(message.Timestamp);
                embedBuilder.WithFooter(Context.Channel.Name);
                if (!string.IsNullOrEmpty(message.Content))
                    embedBuilder.WithDescription(message.Content);

                var attachment = message.Attachments.FirstOrDefault();
                if (attachment == null)
                {
                    await channel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                    return;
                }

                var extension = Path.GetExtension(attachment.Filename);
                if (!_imageExtensions.Any(x => x.Equals(extension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    embedBuilder.WithImageUrl(attachment.Url);
                    await channel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
                    return;
                }

                using var httpClient = new HttpClient();
                var file = await httpClient.GetStreamAsync(new Uri(attachment.Url)).ConfigureAwait(false);
                await channel.SendFileAsync(file, attachment.Filename, string.Empty, embed: embedBuilder.Build()).ConfigureAwait(false);
            }

            private async Task MoveMessageTextAsync(IMessage message, SocketTextChannel channel)
            {
                var author = message.Author as SocketGuildUser;
                var attachment = message.Attachments.FirstOrDefault();

                var msg = $"{author.Nickname()} - {message.Timestamp.DateTime:HH:mm:ss yyyy/MM/dd}  •  {Context.Channel.Name}\n{message.Content}";
                if (attachment == null)
                {
                    await channel.SendMessageAsync(msg).ConfigureAwait(false);
                    return;
                }

                using var httpClient = new HttpClient();
                var file = await httpClient.GetStreamAsync(new Uri(attachment.Url)).ConfigureAwait(false);
                await channel.SendFileAsync(file, attachment.Filename, msg).ConfigureAwait(false);
            }
        }
    }
}