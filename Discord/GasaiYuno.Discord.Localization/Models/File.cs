using Newtonsoft.Json;
using System.Text;

namespace GasaiYuno.Discord.Localization.Models;

internal abstract class File<T>
{
    public static T Read(FileInfo file)
    {
        if (!file.Exists)
            throw new InvalidOperationException("The file does not exist.");

        var json = File.ReadAllText(file.FullName, Encoding.UTF8);
        return JsonConvert.DeserializeObject<T>(json, new DictionaryConverter());
    }

    public static void Write(T data, FileInfo file)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented, new DictionaryConverter());
        File.WriteAllText(file.FullName, json);
    }
}