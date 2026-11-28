using MusicJam.Studio.Helpers;
using MusicJam.Studio.Models.Users;
using MusicJam.Studio.Requests.Auth;
using MusicJam.Studio.Services.TokenService;
using MusicJam.Studio.Storages.UsersStorage;
using Shared;

namespace MusicJam.Studio.Services.AuthService;

internal class AuthService(
    IUserStorage userStorage,
    ITokenService tokenService)
    : IAuthService
{
    public async Task<Result<string, IAuthService.AuthError>> Login(LoginRequest request)
    {
        var user = await userStorage.GetUserByEmail(request.Email);
        if (user is null || !PasswordHelper.Verify(request.Password, user.Password))
        {
            return IAuthService.AuthError.UserNotFound;
        }

        return tokenService.GenerateToken(user);
    }

    public async Task<Result<string, IAuthService.AuthError>> Register(RegisterRequest request)
    {
        var user = await userStorage.GetUserByEmail(request.Email);
        if (user is not null)
        {
            return IAuthService.AuthError.SameEmailAlreadyExists;
        }

        // todo написать тесты для этого случая
        if (!IsStrongPassword(request.Password))
        {
            return IAuthService.AuthError.WeakPassword;
        }

        var passwordHash = PasswordHelper.Hash(request.Password);

        user = new User(request.Email, passwordHash, request.Username);
        var saved = await userStorage.SaveUser(user);

        return tokenService.GenerateToken(saved);
    }

    private static bool IsStrongPassword(string password)
    {
        // todo реализовать логику по проверке силы пароля: какие символы используются, длина, частые паттерны и пр.
        return true;
    }
}
