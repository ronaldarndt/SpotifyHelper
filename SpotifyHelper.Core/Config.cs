using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotifyHelper.Core;

public class ConfigProvider<T>
{
    private const string PATH = @".\config.json";

    public event ConfigChangedHandler? ConfigChanged;
    public delegate void ConfigChangedHandler(T newConfig);

    private static readonly Lazy<JsonSerializerOptions> s_serializerOptions = new(() => new JsonSerializerOptions()
    {
        AllowTrailingCommas = false,
        WriteIndented = false,
        PropertyNamingPolicy = null,
        ReadCommentHandling = JsonCommentHandling.Skip
    });

    public T Config { get; private set; }

    public ConfigProvider(T defaultConfig)
    {
        if (File.Exists(PATH))
        {
            using var file = File.OpenRead(PATH);

            Config = JsonSerializer.Deserialize<T>(file, s_serializerOptions.Value)!;
        }
        else
        {
            Config = defaultConfig;
        }
    }

    public async Task UpdateAsync(T newConfig)
    {
        if (Config is null || Config.Equals(newConfig))
        {
            return;
        }

        Config = newConfig;
        ConfigChanged?.Invoke(Config);

        using var file = File.OpenWrite(PATH);

        await JsonSerializer.SerializeAsync(file, newConfig, s_serializerOptions.Value);
    }
}
