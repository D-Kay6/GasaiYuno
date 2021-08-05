using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GasaiYuno.Discord.Extensions
{
    public static class UserExtensions
    {
        private static readonly Random Random = new();

        public static List<SocketGuildUser> GetUsers(this ISocketMessageChannel channel)
        {
            var users = new List<SocketGuildUser>();
            channel.GetUsersAsync().ForEachAsync(u => users.AddRange(u.Cast<SocketGuildUser>()));
            return users;
        }

        public static SocketGuildUser GetRandomUser(this ISocketMessageChannel channel)
        {
            var users = channel.GetUsers();
            users.RemoveAll(u => u.IsBot);
            return users[Random.Next(users.Count)];
        }

        public static SocketGuildUser GetUser(this ISocketMessageChannel channel, string name)
        {
            return channel.GetUsers().GetUser(name);
        }

        public static SocketGuildUser GetUser(this IEnumerable<SocketGuildUser> list, string name)
        {
            return list.FirstOrDefault(u => name.Contains(u.Id.ToString())) ??
                   list.FirstOrDefault(u => u.Username.Equals(name, StringComparison.CurrentCultureIgnoreCase)) ??
                   list.Where(u => !string.IsNullOrEmpty(u.Nickname)).FirstOrDefault(u =>
                       u.Nickname.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static string Nickname(this IGuildUser user)
        {
            return user.Nickname ?? user.Username ?? "user";
        }
    }
}