using System.Text.RegularExpressions;
using GasaiYuno.Discord.Core.Interfaces;

namespace GasaiYuno.Discord.Models;

internal class Localization : ILocalization
{
    private readonly Dictionary<string, string> _translations;

    internal Localization(Dictionary<string, string> translations)
    {
        _translations = translations;
    }

    internal void Merge(Localization localization)
    {
        foreach (var (key, value) in localization._translations)
            _translations.Add(key, value);
    }

    public string Translate(string code) => Translate(code, '.');
    public string Translate(string code, char separator)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("No valid code was provided.", nameof(code));

        if (!_translations.TryGetValue(code, out var result))
            throw new ArgumentOutOfRangeException(nameof(code), "No message could be found with this code.");

        return Regex.Replace(result, @"\s*\[enter\]\s*", Environment.NewLine);
    }

    public string Translate(string code, params object[] objects) => Translate(code, '.', objects);
    public string Translate(string code, char separator, params object[] objects)
    {
        var result = Translate(code, separator);
        if (objects?.Length > 0)
            result = string.Format(result, objects);

        return result;
    }
}