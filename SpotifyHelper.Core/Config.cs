using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotifyHelper.Core
{
    public class ConfigProvider<T>
    {
        private const string PATH = @".\config.json";

        public static event ConfigChangedHandler ConfigChanged;
        public delegate void ConfigChangedHandler(T newConfig);

        private static Lazy<JsonSerializerOptions> s_serializerOptions = new Lazy<JsonSerializerOptions>(() => new JsonSerializerOptions()
        {
            AllowTrailingCommas = false,
            WriteIndented = false,
            PropertyNamingPolicy = null,
            ReadCommentHandling = JsonCommentHandling.Skip
        });

        private static T _config;

        public static T Config
        {
            get => _config;

            private set
            {
                _config = value;

                ConfigChanged.Invoke(value);
            }
        }

        public static async Task InitializeAsync(T defaultConfig)
        {
            if (File.Exists(PATH))
            {
                using var file = File.OpenRead(PATH);

                Config = await JsonSerializer.DeserializeAsync<T>(file, s_serializerOptions.Value);
            }
            else
            {
                Config = defaultConfig;
            }
        }

        public static async Task UpdateAsync(T newConfig)
        {
            if (!Config.Equals(newConfig))
            {
                Config = newConfig;

                using var file = File.OpenWrite(PATH);

                await JsonSerializer.SerializeAsync(file, newConfig, s_serializerOptions.Value);
            }
        }
    }
}
