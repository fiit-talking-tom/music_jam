using MusicJam.Studio.Requests.Auth;
using MusicJam.Studio.Services.AuthService;
using Shouldly;
using Tests.FunctionalTests.Extensions;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.AuthServiceTests;

[Collection(TestCollection.Name)]
public class RegistrationTests(TestApplicationFixture fixture) : AuthServiceTestBase(fixture)
{
    private readonly RegisterRequest _registerRequest = new()
    {
        Username = TestUserName,
        Email = TestUserEmail,
        Password = TestUserPassword,
    };

    [Fact]
    public async Task Register_WhenUserDoesNotExist_CreateUserAndReturnsAccessToken()
    {
        var result = await AuthService.Register(_registerRequest);

        result.IsSuccess.ShouldBeTrue();

        var accessToken = result.GetValueOrThrow();
        accessToken.ShouldNotBeNullOrWhiteSpace();
        accessToken.ShouldBeValidAccessToken();
    }

    [Fact]
    public async Task Register_WhenUserExists_ReturnsSameUserAlreadyExists()
    {
        await CreateUserAsync(TestUserEmail, TestUserPassword);

        var result = await AuthService.Register(_registerRequest);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IAuthService.AuthError.SameEmailAlreadyExists);
    }

    [Theory]
    [InlineData("123", false)]
    [InlineData("qwerty", false)]
    [InlineData("qwerty123", false)]
    [InlineData("1234567890", false)]
    [InlineData("S500ERmbXrgp", true)]
    [InlineData("3BjfOe-%kHZK", true)]
    public async Task Register_WhenWeakPassword_ReturnsWeakPassword(string testPassword, bool isSuccess)
    {
        var request = new RegisterRequest
        {
            Username = TestUserName,
            Password = testPassword,
            Email = $"TestUserEmail_{testPassword}"
        };

        var result = await AuthService.Register(request);

        result.IsSuccess.ShouldBe(isSuccess);

        if (!isSuccess)
        {
            result.GetFaultOrThrow().ShouldBe(IAuthService.AuthError.WeakPassword);
        }
    }
}
