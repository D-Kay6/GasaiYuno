using Discord.Commands;
using GasaiYuno.Discord.Core.Extensions;
using GasaiYuno.Discord.Domain;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Commands.TypeReaders
{
    public class PollOptionTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Task.FromResult(TypeReaderResult.FromError(new InvalidDataException("The input was empty.")));

            var options = input.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            var emojis = EmojiExtensions.Get(options.Length);

            var pollOptions = new PollOption[options.Length];
            for (var i = 0; i < options.Length; i++)
            {
                pollOptions[i] = new PollOption
                {
                    Emote = emojis[i].Name,
                    Message = options[i]
                };
            }

            return Task.FromResult(TypeReaderResult.FromSuccess(pollOptions));
        }
    }
}