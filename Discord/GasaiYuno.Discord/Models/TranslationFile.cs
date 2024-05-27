using Newtonsoft.Json;

namespace GasaiYuno.Discord.Models;

internal static class TranslationFile
{
    public static Localization Read(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found.", filePath);

        var fileContent = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileContent, new DictionaryConverter());
        var translations = new Dictionary<string, string>();
        ConvertData(data, ref translations);
        return new Localization(translations);
    }

    private static void ConvertData(Dictionary<string, object> data, ref Dictionary<string, string> translations, string parentName = null)
    {
        foreach (var (key, value) in data)
        {
            var name = key;
            if (!string.IsNullOrWhiteSpace(parentName))
                name = $"{parentName}.{key}";

            if (value is Dictionary<string, object> dictionary)
            {
                ConvertData(dictionary, ref translations, name);
                continue;
            }

            translations.Add(name, value.ToString());
        }
    }
}