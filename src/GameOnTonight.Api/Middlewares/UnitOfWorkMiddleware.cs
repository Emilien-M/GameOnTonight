using GameOnTonight.Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace GameOnTonight.Api.Middlewares;

/// <summary>
/// Middleware responsible for committing the UnitOfWork at the end of each successful HTTP request.
/// This ensures that handlers do not call SaveChanges manually.
/// </summary>
public sealed class UnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;

    public UnitOfWorkMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Execute the rest of the pipeline
        await _next(context);

        // Commit only after successful execution (no exception) and for non-GET/HEAD requests
        // It's safe to call SaveChanges even when there are no changes; EF will no-op.
        if (!HttpMethods.IsGet(context.Request.Method) &&
            !HttpMethods.IsHead(context.Request.Method) &&
            !HttpMethods.IsOptions(context.Request.Method) &&
            !HttpMethods.IsTrace(context.Request.Method) &&
            context.Response.StatusCode < 400)
        {
            var uow = context.RequestServices.GetService<IUnitOfWork>();
            if (uow is not null)
            {
                await uow.SaveChangesAsync();
            }
        }
    }
}
