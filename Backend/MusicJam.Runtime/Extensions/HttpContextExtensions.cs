using MusicJam.Runtime.Constants;

namespace MusicJam.Runtime.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetSessionId(this HttpContext context) =>
        context.TryGetSessionId()
        ?? throw new InvalidOperationException("SessionMiddleware не подключена");

    public static Guid? TryGetSessionId(this HttpContext context) =>
        context.Items[SessionCookie.Name] is Guid sessionId ? sessionId : null;
}
