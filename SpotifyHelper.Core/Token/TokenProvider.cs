using System;
using System.Threading;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyHelper.Core.Utils;

namespace SpotifyHelper.Core.Token;

public class TokenProvider : ITokenProvider
{
    private readonly Uri s_baseUri = new("http://localhost:3030");
    private static readonly string[] s_scopes = new[]
    {
        Scopes.PlaylistReadPrivate,
        Scopes.PlaylistReadCollaborative,
        Scopes.PlaylistModifyPublic,
        Scopes.PlaylistModifyPrivate,
        Scopes.UserReadPlaybackState,
        Scopes.UserReadCurrentlyPlaying
    };

    private readonly string m_clientId;

    public TokenProvider(string clientId)
    {
        m_clientId = clientId;
    }

    public async Task<PKCETokenResponse> GetPKCETokenAsync()
    {
        var (verifier, challenge) = PKCEUtil.GenerateCodes(120);

        var code = await GetCodeAsync(challenge);

        return await new OAuthClient()
            .RequestToken(new PKCETokenRequest(m_clientId, code, s_baseUri, verifier));
    }

    public string GetClientId()
    {
        return m_clientId;
    }

    private async Task<string> GetCodeAsync(string challenge)
    {
        var loginUrl = GetLoginUrl(challenge);

        using var server = CreateServer();
        using var loginBrowser = Browser.OpenDefault(loginUrl.AbsoluteUri);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));

        await server.Start();

        var code = await WaitForCodeAsync(server, cts.Token);

        await server.Stop();

        return code;
    }

    private async Task<string> WaitForCodeAsync(EmbedIOAuthServer server, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        var handler = (object sender, AuthorizationCodeResponse response) =>
        {
            tcs.TrySetResult(response.Code);
            return Task.CompletedTask;
        };

        server.AuthorizationCodeReceived += handler;

        using var _ = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

        var code = await tcs.Task;

        server.AuthorizationCodeReceived -= handler;

        return code;
    }

    private Uri GetLoginUrl(string challenge)
    {
        var loginRequest = new LoginRequest(s_baseUri, m_clientId, LoginRequest.ResponseType.Code)
        {
            CodeChallengeMethod = "S256",
            CodeChallenge = challenge,
            Scope = s_scopes,
            ShowDialog = false
        };

        return loginRequest.ToUri();
    }

    private EmbedIOAuthServer CreateServer()
    {
        return new(s_baseUri, 3030);
    }
}

