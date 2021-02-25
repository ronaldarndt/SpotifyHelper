using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyHelper.Core.Extensions;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyHelper.Core
{
    public static class Auth
    {
        private static Uri s_baseUri = new Uri("http://localhost:3030");
        private const string CLIENT_ID = "d6f591b2fcea4e32ba81486eb246d49d";

        public static async Task<IAuthenticator> GetAuthenticatorFromFileAsync()
        {
            var token = await GetFromFileAsync();

            if (token is null)
            {
                return null;
            }

            return await GetAuthenticatorFromTokenAsync(token);
        }

        public static async Task<IAuthenticator> GetAuthenticatorAsync()
        {
            var token = await GetFromFileAsync();

            if (token is null)
            {
                var (verifier, challenge) = PKCEUtil.GenerateCodes(120);

                var code = await GetCodeAsync(challenge);

                token = await new OAuthClient()
                    .RequestToken(new PKCETokenRequest(CLIENT_ID, code, s_baseUri, verifier));
            }

            return await GetAuthenticatorFromTokenAsync(token);
        }

        private static async Task<IAuthenticator> GetAuthenticatorFromTokenAsync(PKCETokenResponse token)
        {
            var authenticator = new PKCEAuthenticator(CLIENT_ID, token);

            await WriteToFileAsync(token);

            authenticator.TokenRefreshed += async (sender, e) =>
            {
                await WriteToFileAsync(e);
            };

            return authenticator;
        }

        private static async Task<string> GetCodeAsync(string challenge)
        {
            var loginUrl = GetLoginUrl(challenge);

            var code = string.Empty;

            var server = CreateServer();
            await server.Start();

            var loginBrowser = Browser.OpenDefault(loginUrl.AbsoluteUri);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));

            server.AuthorizationCodeReceived += (sender, receivedCode) =>
            {
                code = receivedCode.Code;

                loginBrowser?.CloseMainWindow();

                return Task.CompletedTask;
            };

            await loginBrowser.WaitForExitAsync(cts.Token);

           await Cleanup();

            return code;

            async Task Cleanup()
            {            
                loginBrowser?.Dispose();
                loginBrowser = null;

                await server?.Stop();
                server?.Dispose();
                server = null;

                cts?.Dispose();
                cts = null;
            }
        }

        private static Uri GetLoginUrl(string challenge)
        {
            var scopes = new[]
            {
                Scopes.PlaylistReadPrivate,
                Scopes.PlaylistReadCollaborative,
                Scopes.PlaylistModifyPublic,
                Scopes.PlaylistModifyPrivate,
                Scopes.UserReadPlaybackState,
                Scopes.UserReadCurrentlyPlaying
            };

            var loginRequest = new LoginRequest(s_baseUri, CLIENT_ID, LoginRequest.ResponseType.Code)
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = scopes,
                ShowDialog = false
            };

            return loginRequest.ToUri();
        }

        private static EmbedIOAuthServer CreateServer() => new EmbedIOAuthServer(s_baseUri, 3030);

        private static async Task WriteToFileAsync(PKCETokenResponse obj)
        {
            await using var file = File.OpenWrite("creds.dat");

            await JsonSerializer.SerializeAsync(file, obj);

            await file.FlushAsync();
        }

        private static async Task<PKCETokenResponse> GetFromFileAsync()
        {
            try
            {
                await using var file = File.OpenRead("creds.dat");

                var result = await JsonSerializer.DeserializeAsync(file, typeof(PKCETokenResponse));

                return (PKCETokenResponse)result;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
