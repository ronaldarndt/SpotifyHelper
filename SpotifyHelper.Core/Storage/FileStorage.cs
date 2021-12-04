using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotifyHelper.Core.Storage;

public class FileStorage : IPersistentStorage
{
    private static readonly JsonSerializerOptions s_serializerOptions = new()
    {
        AllowTrailingCommas = false,
        WriteIndented = false,
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public async Task<T?> GetAsync<T>(string name)
    {
        await using var stream = GetFile(name);

        if (stream is null)
        {
            return default;
        }

        return await JsonSerializer.DeserializeAsync<T?>(stream, s_serializerOptions);
    }

    public T? Get<T>(string name)
    {
        using var stream = GetFile(name);

        if (stream is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T?>(stream, s_serializerOptions);
    }

    public async Task SaveAsync<T>(string name, T content)
    {
        await using var file = File.OpenWrite(name);

        await JsonSerializer.SerializeAsync(file, content, s_serializerOptions);

        await file.FlushAsync();
    }

    public bool Exists(string name)
    {
        return File.Exists(name);
    }

    private FileStream? GetFile(string name)
    {
        try
        {
            return File.OpenRead(name);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
}

