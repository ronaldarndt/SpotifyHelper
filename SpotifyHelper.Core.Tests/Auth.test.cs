using System.Threading.Tasks;
using Moq;
using SpotifyAPI.Web;
using SpotifyHelper.Core.Storage;
using SpotifyHelper.Core.Tests.Storage;
using SpotifyHelper.Core.Token;
using Xunit;

namespace SpotifyHelper.Core.Tests;

public class AuthTest
{
    [Fact]
    public async Task GetAuthenticatorFromFileAsync_FileDoesntExist_ReturnsNullAsync()
    {
        // Arrange
        var storage = new Mock<IPersistentStorage>();
        var token = new Mock<ITokenProvider>();
        token.Setup(x => x.GetClientId()).Returns("");
        var auth = new Auth(storage.Object, token.Object);

        // Act
        var file = await auth.GetAuthenticatorFromFileAsync();

        // Assert
        Assert.Null(file);
        token.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAuthenticatorFromFileAsync_FileExists_ReturnsAuthenticatorAsync()
    {
        // Arrange
        var storage = new MemoryStorage();
        var tokenProvider = new Mock<ITokenProvider>();
        tokenProvider.Setup(x => x.GetClientId()).Returns("");
        var auth = new Auth(storage, tokenProvider.Object);

        var token = new PKCETokenResponse()
        {
            AccessToken = "abc",
            ExpiresIn = 60 * 1000,
            RefreshToken = "def",
            TokenType = "Bearer"
        };

        await storage.SaveAsync("creds.dat", token);

        // Act
        var authenticator = await auth.GetAuthenticatorFromFileAsync();

        // Assert
        Assert.NotNull(authenticator);
        Assert.IsAssignableFrom<IAuthenticator>(authenticator);
        tokenProvider.Verify(x => x.GetPKCETokenAsync(), Times.Never());
    }

    [Fact]
    public async Task GetAuthenticatorAsync_FileExists_ReturnsAuthenticatorAsync()
    {
        // Arrange
        var storage = new MemoryStorage();
        var tokenProvider = new Mock<ITokenProvider>();
        tokenProvider.Setup(x => x.GetClientId()).Returns("");
        var auth = new Auth(storage, tokenProvider.Object);

        var token = new PKCETokenResponse()
        {
            AccessToken = "abc",
            ExpiresIn = 60 * 1000,
            RefreshToken = "def",
            TokenType = "Bearer"
        };

        await storage.SaveAsync("creds.dat", token);

        // Act
        var authenticator = await auth.GetAuthenticatorAsync();

        // Assert
        Assert.NotNull(authenticator);
        Assert.IsAssignableFrom<IAuthenticator>(authenticator);
        tokenProvider.Verify(x => x.GetPKCETokenAsync(), Times.Never());
    }

    [Fact]
    public async Task GetAuthenticatorAsync_FileDoesntExists_ReturnsAuthenticator_CallsGetTokenAsync()
    {
        // Arrange
        var storage = new Mock<IPersistentStorage>();
        var tokenProvider = new Mock<ITokenProvider>();
        tokenProvider.Setup(x => x.GetClientId()).Returns("");
        tokenProvider
            .Setup(x => x.GetPKCETokenAsync())
            .Returns(Task.FromResult(new PKCETokenResponse()));

        var auth = new Auth(storage.Object, tokenProvider.Object);

        // Act
        var authenticator = await auth.GetAuthenticatorAsync();

        // Assert
        Assert.NotNull(authenticator);
        Assert.IsAssignableFrom<IAuthenticator>(authenticator);
        tokenProvider.Verify(x => x.GetPKCETokenAsync(), Times.Once());
    }
}

