using Discord.Commands;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Core.Commands.TypeReaders
{
    public class TimeSpanTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Task.FromResult(TypeReaderResult.FromError(new InvalidDataException("The input was empty.")));

            var duration = TimeSpan.Zero;
            var values = input.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in values)
            {
                var match = Regex.Match(value, @"\d+");
                if (!match.Success)
                    return Task.FromResult(TypeReaderResult.FromError(new FormatException("A time value was not formatted correctly.")));

                var amount = int.Parse(match.Value);
                var unit = value.Substring(match.Length, 1);

                switch (unit.ToLower())
                {
                    case "m":
                        duration += TimeSpan.FromMinutes(amount);
                        break;
                    case "h":
                        duration += TimeSpan.FromHours(amount);
                        break;
                    case "d":
                        duration += TimeSpan.FromDays(amount);
                        break;
                    case "w":
                        duration += TimeSpan.FromDays(amount * 7);
                        break;
                    default:
                        duration += TimeSpan.FromDays(amount);
                        break;
                }
            }

            return Task.FromResult(duration != TimeSpan.Zero
                ? TypeReaderResult.FromSuccess(duration)
                : TypeReaderResult.FromError(new FormatException("The input could not be converted to a duration.")));
        }
    }
}