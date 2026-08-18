
using Application.Abstractions;
using Serilog;

namespace InvoicesService.Middleware;

public class MultiTenacyMiddleware(
    RequestDelegate next,
    ITenantProvider tenantProvider)
{

    public async Task InvokeAsync(HttpContext context)
    {
        Log.Information("Invoking Multi-Tenacy Middleware");
        
        var compId = context.User.Claims.FirstOrDefault(c => c.Type == "mf:cid")?.Value;

        if (!Guid.TryParse(compId, out var companyId))
        {
            throw new UnauthorizedAccessException("Invalid company id");
        }
        tenantProvider.SetTenantId(companyId);

        await next(context);
    }
}