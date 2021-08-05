using System;
using System.Text.RegularExpressions;

namespace GasaiYuno.Discord.Extensions
{
    public static class StringExtensions
    {
        public static string ToPossessive(this string item)
        {
            var s = item.EndsWith("s", StringComparison.CurrentCultureIgnoreCase) ? "" : "s";
            return $"{item}'{s}";
        }

        public static TimeSpan GetDuration(this string item)
        {
            var duration = TimeSpan.Zero;

            var match = Regex.Match(item, @"\d+s");
            if (match.Success) duration += TimeSpan.FromSeconds(int.Parse(Regex.Match(match.Value, @"\d+").Value));
            match = Regex.Match(item, @"\d+m");
            if (match.Success) duration += TimeSpan.FromMinutes(int.Parse(Regex.Match(match.Value, @"\d+").Value));
            match = Regex.Match(item, @"\d+h");
            if (match.Success) duration += TimeSpan.FromHours(int.Parse(Regex.Match(match.Value, @"\d+").Value));
            match = Regex.Match(item, @"\d+d");
            if (match.Success) duration += TimeSpan.FromDays(int.Parse(Regex.Match(match.Value, @"\d+").Value));

            if (duration != TimeSpan.Zero) return duration;

            match = Regex.Match(item, @"\d+");
            if (match.Success) duration += TimeSpan.FromDays(int.Parse(Regex.Match(match.Value, @"\d+").Value));
            return duration;
        }
    }
}