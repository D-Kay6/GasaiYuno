using GasaiYuno.Interface.Storage;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GasaiYuno.Storage.Configuration
{
    public class ConfigService : IConfigStorage
    {
        /// <summary>The folder name where the file is located.</summary>
        private const string FolderName = "Configuration";

        /// <inheritdoc/>
        public T Read<T>() where T : struct
        {
            if (!Directory.Exists(FolderName))
                Directory.CreateDirectory(FolderName);

            T config;
            var path = Path.Combine(FolderName, $"{typeof(T).Name}.json");
            if (!File.Exists(path))
            {
                config = Activator.CreateInstance<T>();
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            else
            {
                var json = File.ReadAllText(path);
                config = JsonConvert.DeserializeObject<T>(json);
            }

            return config;
        }

        /// <inheritdoc/>
        public T Read<T>(string name) where T : struct
        {
            if (!Directory.Exists(FolderName))
                Directory.CreateDirectory(FolderName);

            T config;
            var path = Path.Combine(FolderName, $"{name}.json");
            if (!File.Exists(path))
            {
                config = Activator.CreateInstance<T>();
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            else
            {
                var json = File.ReadAllText(path);
                config = JsonConvert.DeserializeObject<T>(json);
            }

            return config;
        }
    }
}