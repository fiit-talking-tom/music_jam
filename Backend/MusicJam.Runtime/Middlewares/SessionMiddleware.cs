using MusicJam.Runtime.Constants;
using MusicJam.Runtime.Hubs;

namespace MusicJam.Runtime.Middlewares;

public sealed class SessionMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        if (!Guid.TryParse(context.Request.Cookies[SessionCookie.Name], out var sessionId))
        {
            var path = context.Request.Path;
            if (path == GameHub.Path || path.StartsWithSegments(GameHub.Path + "/negotiate"))
            {
                return next(context);
            }

            sessionId = Guid.NewGuid();
            context.Response.Cookies.Append(SessionCookie.Name, sessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/runtime",
                MaxAge = TimeSpan.FromHours(12),
            });
        }

        context.Items[SessionCookie.Name] = sessionId;
        return next(context);
    }
}