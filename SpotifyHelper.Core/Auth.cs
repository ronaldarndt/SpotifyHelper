using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyHelper.Core.Extensions;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpotifyHelper.Core
{
    public static class Auth
    {
        private static Uri s_baseUri = new Uri("http://localhost:3030");
        private const string CLIENT_ID = "d6f591b2fcea4e32ba81486eb246d49d";

        public static async Task<IAuthenticator> GetAuthenticatorFromFileAsync()
        {
            var token = await GetFromFile();

            if (token is null)
            {
                return null;
            }

            return await GetAuthenticatorFromToken(token);
        }

        public static async Task<IAuthenticator> GetAuthenticator()
        {
            var token = await GetFromFile();

            if (token is null)
            {
                var (verifier, challenge) = PKCEUtil.GenerateCodes(120);

                var code = await GetCode(challenge);

                token = await new OAuthClient().RequestToken(new PKCETokenRequest(CLIENT_ID, code, s_baseUri, verifier));
            }

            return await GetAuthenticatorFromToken(token);
        }

        private static async Task<IAuthenticator> GetAuthenticatorFromToken(PKCETokenResponse token)
        {
            var authenticator = new PKCEAuthenticator(CLIENT_ID, token);

            await WriteToFile(token);

            authenticator.TokenRefreshed += async (sender, e) =>
            {
                await WriteToFile(e);
            };

            return authenticator;
        }

        private static async Task<string> GetCode(string challenge)
        {
            var loginUrl = GetLoginUrl(challenge);

            var code = "";

            var server = CreateServer();
            await server.Start();

            try
            {
                var loginBrowser = Browser.OpenDefault(loginUrl.AbsoluteUri);

                server.AuthorizationCodeReceived += async (sender, receivedCode) =>
                {
                    code = receivedCode.Code;

                    loginBrowser.CloseMainWindow();

                    await server.Stop();
                };

                loginBrowser.Refresh();

                await loginBrowser.WaitForExitAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return code;
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
                Scope = scopes
            };

            return loginRequest.ToUri();
        }

        private static EmbedIOAuthServer CreateServer() => new EmbedIOAuthServer(s_baseUri, 3030);

        private static async Task WriteToFile(object obj)
        {
            using var file = File.OpenWrite("creds.dat");

            await JsonSerializer.SerializeAsync(file, obj);

            await file.FlushAsync();
        }

        private static async Task<PKCETokenResponse> GetFromFile()
        {
            try
            {
                using var file = File.OpenRead("creds.dat");

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
