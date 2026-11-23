using MusicJam.Runtime.Extensions;
using MusicJam.Runtime.Storages.BindingsStorage;

namespace MusicJam.Runtime.Middlewares;

public class EnrichBindingContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IBindingsStorage bindingsStorage)
    {
        var sessionId = context.GetSessionId();

        var binding = await bindingsStorage.GetBinding(sessionId);
        if (binding is not null)
        {
            context.Features.Set(binding);
        }

        await next(context);
    }
}
