using GasaiYuno.Interface.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GasaiYuno.Localization.Models
{
    internal class Translation : File<Translation>, ITranslation
    {
        private readonly Random _random;

        public Dictionary<string, object> Messages { get; init; }
        public List<string> GroupPraises { get; init; }
        public List<string> UserPraises { get; init; }

        public Translation()
        {
            _random = new Random();
        }

        public string Message(params string[] path)
        {
            if (path == null || path.Length == 0)
                throw new ArgumentException("No valid path was provided.", nameof(path));

            object result = Messages;
            foreach (var part in path)
            {
                if (result is not Dictionary<string, object> collection)
                    throw new ArgumentOutOfRangeException(nameof(path), "No message could be found with this path.");

                result = collection[part];
            }

            if (result == null)
                throw new ArgumentOutOfRangeException(nameof(path), "No message could be found with this path.");

            return result as string;
        }

        public string Message(string[] path, params object[] objects)
        {
            var message = Message(path);
            return string.Format(message, objects);
        }

        public string Message(string path, params object[] objects) => Message(path, '.', objects);
        public string Message(string path, char separator, params object[] objects)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("No valid path was provided.", nameof(path));

            var pathParts = path.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var result = Message(pathParts, objects);
            return Regex.Replace(result, @"\s*\[enter\]\s*", Environment.NewLine);
        }

        public string UserPraise(params object[] objects)
        {
            if (!UserPraises.Any())
                throw new InvalidOperationException("No praises have been added to the translation file.");

            var index = _random.Next(UserPraises.Count);
            return string.Format(UserPraises[index], objects);
        }

        public string GroupPraise(params object[] objects)
        {
            if (!GroupPraises.Any())
                throw new InvalidOperationException("No praises have been added to the translation file.");

            var index = _random.Next(GroupPraises.Count);
            return string.Format(GroupPraises[index], objects);
        }
    }
}