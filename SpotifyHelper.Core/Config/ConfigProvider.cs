using System;
using System.IO;
using System.Threading.Tasks;
using SpotifyHelper.Core.Storage;

namespace SpotifyHelper.Core.Config;

public class ConfigProvider<T>
{
    public event Action<T>? ConfigChanged;
    public T Config { get; private set; }

    private const string PATH = @".\config.json";

    private readonly IPersistentStorage m_persistentStorage;

    public ConfigProvider(IPersistentStorage persistentStorage)
    {
        m_persistentStorage = persistentStorage;
        Config = persistentStorage.Get<T>(PATH) ?? throw new FileNotFoundException("config.json not found.");
    }

    public async Task UpdateAsync(T newConfig)
    {
        if (Config!.Equals(newConfig))
        {
            return;
        }

        Config = newConfig;
        ConfigChanged?.Invoke(Config);

        await m_persistentStorage.SaveAsync(PATH, newConfig);
    }
}
