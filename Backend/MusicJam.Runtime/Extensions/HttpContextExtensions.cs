using MusicJam.Runtime.Constants;

namespace MusicJam.Runtime.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetSessionId(this HttpContext context)
    {
        context.Items.TryGetValue(SessionCookie.Name, out var cookieValue);
        if (cookieValue is Guid sessionId)
        {
            return sessionId;
        }

        throw new InvalidOperationException("Отсутствует game.sid");
    }
}
