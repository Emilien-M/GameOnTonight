using GameOnTonight.Domain.Repositories;

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
        var uow = context.RequestServices.GetRequiredService<IUnitOfWork>();
        var alreadyRollBack = false;
        
        using var transactionalElement = await uow.BeginTransactionAsync();
        
        try
        {
            await _next(context);

            if (!HttpMethods.IsGet(context.Request.Method) &&
                !HttpMethods.IsHead(context.Request.Method) &&
                !HttpMethods.IsOptions(context.Request.Method) &&
                !HttpMethods.IsTrace(context.Request.Method) &&
                context.Response.StatusCode < 400)
            {
                await transactionalElement.CommitAsync();
            }
            else
            {
                alreadyRollBack = true;
                await transactionalElement.RollbackAsync();
            }
        }
        catch (Exception)
        {
            if (!alreadyRollBack)
                await transactionalElement.RollbackAsync();
            throw;
        }
    }
}
