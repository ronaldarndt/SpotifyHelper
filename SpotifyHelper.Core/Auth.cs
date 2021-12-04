using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyHelper.Core.Storage;
using SpotifyHelper.Core.Token;

namespace SpotifyHelper.Core;

public class Auth
{
    private readonly IPersistentStorage m_persistentStorage;
    private readonly ITokenProvider m_tokenProvider;
    private const string FILENAME = "creds.dat";

    public Auth(IPersistentStorage persistentStorage, ITokenProvider tokenProvider)
    {
        m_persistentStorage = persistentStorage;
        m_tokenProvider = tokenProvider;
    }

    public async Task<IAuthenticator?> GetAuthenticatorFromFileAsync()
    {
        var token = await m_persistentStorage.GetAsync<PKCETokenResponse>(FILENAME);

        return token is null
            ? null
            : await GetAuthenticatorFromTokenAsync(token);
    }

    public async Task<IAuthenticator> GetAuthenticatorAsync()
    {
        var token = await m_persistentStorage.GetAsync<PKCETokenResponse>(FILENAME);

        if (token is null)
        {
            token = await m_tokenProvider.GetPKCETokenAsync();
        }

        return await GetAuthenticatorFromTokenAsync(token);
    }

    private async Task<IAuthenticator> GetAuthenticatorFromTokenAsync(PKCETokenResponse token)
    {
        var authenticator = new PKCEAuthenticator(m_tokenProvider.GetClientId(), token);

        await m_persistentStorage.SaveAsync(FILENAME, token);

        authenticator.TokenRefreshed += async (sender, e) =>
        {
            await m_persistentStorage.SaveAsync(FILENAME, e);
        };

        return authenticator;
    }
}

