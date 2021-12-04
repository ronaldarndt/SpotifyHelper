using SpotifyHelper.Core.Storage;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotifyHelper.Core.Tests.Storage;

internal class MemoryStorage : IPersistentStorage
{
    private readonly Dictionary<string, string> m_memory = new();

    public Task<T?> GetAsync<T>(string name)
    {
        return Task.FromResult(Get<T>(name));
    }

    public T? Get<T>(string name)
    {
        if (m_memory.TryGetValue(name, out var value))
        {
            return JsonSerializer.Deserialize<T>(value);
        }

        return default;
    }

    public Task SaveAsync<T>(string name, T content)
    {
        var str = JsonSerializer.Serialize(content);

        m_memory.TryAdd(name, str);

        return Task.CompletedTask;
    }

    public bool Exists(string name)
    {
        return m_memory.ContainsKey(name);
    }
}

