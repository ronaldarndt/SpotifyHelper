using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace SpotifyHelper.Core.Token;

public interface ITokenProvider
{
    public Task<PKCETokenResponse> GetPKCETokenAsync();
    public string GetClientId();
}
