using System.Security.Claims;

namespace Shared.Auth;

public static class ClaimsPrincipalExtensions
{
    // todo veselkov.d вызывать на уровне Middlewares и в контроллерах использовать User.Id
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claimValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(claimValue, out var userId))
        {
            throw new InvalidOperationException("User ID is missing or invalid");
        }

        return userId;
    }
}
