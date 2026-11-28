using Microsoft.AspNetCore.Identity;
using Zxcvbn;

namespace MusicJam.Studio.Helpers;

internal static class PasswordHelper
{
    private const int MinPasswordStrength = 2; // из 4 

    private static readonly PasswordHasher<object> Hasher = new();

    public static bool Verify(string password, string hash)
    {
        return Hasher.VerifyHashedPassword(null!, hash, password)
            is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }

    public static string Hash(string password)
    {
        return Hasher.HashPassword(null!, password);
    }

    public static bool IsStrongPassword(string password) =>
        Core.EvaluatePassword(password).Score >= MinPasswordStrength;
}
