using System.Threading.Tasks;

namespace SpotifyHelper.Core.Storage;

public interface IPersistentStorage
{
    Task<T?> GetAsync<T>(string name);
    T? Get<T>(string name);
    Task SaveAsync<T>(string name, T content);
    bool Exists(string name);
}

