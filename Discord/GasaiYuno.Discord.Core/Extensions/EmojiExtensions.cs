using Discord;
using System;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Core.Extensions
{
    public static class EmojiExtensions
    {
        public static Emoji GetNumber(int number)
        {
            if (number is < 1 or > 9)
                throw new ArgumentOutOfRangeException(nameof(number));
            
            return new Emoji($"{number}\uFE0F\u20E3");
        }

        public static List<Emoji> GetNumbers(int count)
        {
            if (count > 10)
                throw new ArgumentOutOfRangeException(nameof(count));

            var emotes = new List<Emoji>();
            for (var i = 1; i <= count; i++)
            {
                emotes.Add(new Emoji($"{i}\uFE0F\u20E3"));
            }

            return emotes;
        }

        public static Emoji GetLetter(int index)
        {
            if (index is < 1 or > 26)
                throw new ArgumentOutOfRangeException(nameof(index));

            return new Emoji(char.ConvertFromUtf32(index + 127461));
        }

        public static Emoji GetLetter(char letter)
        {
            if (letter is (< 'a' or > 'z') and (< 'A' or 'Z'))
                throw new ArgumentOutOfRangeException(nameof(letter));

            var index = letter % 32 + 127461;
            return new Emoji(char.ConvertFromUtf32(index));
        }

        public static List<Emoji> GetLetters(int count)
        {
            if (count is < 1 or > 26)
                throw new ArgumentOutOfRangeException(nameof(count));

            var emotes = new List<Emoji>();
            for (var i = 1; i <= count; i++)
            {
                emotes.Add(new Emoji(char.ConvertFromUtf32(i + 127461)));
            }

            return emotes;
        }

        public static List<Emoji> Get(int count) => count > 9 ? GetLetters(count) : GetNumbers(count);
    }
}