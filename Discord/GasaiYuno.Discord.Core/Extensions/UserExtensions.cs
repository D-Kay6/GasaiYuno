using Discord;
using System;

namespace GasaiYuno.Discord.Core.Extensions
{
    public static class UserExtensions
    {
        public static string Nickname(this IGuildUser user)
        {
            return user.Nickname ?? user.Username ?? "user";
        }

        public static string ToPossessive(this IGuildUser user)
        {
            var name = user.Nickname();
            var s = name.EndsWith("s", StringComparison.CurrentCultureIgnoreCase) ? "" : "s";
            return $"{name}'{s}";
        }
    }
}