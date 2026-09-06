
using Application.Abstractions;
using Serilog;

namespace InvoicesService.Middleware;

public class MultiTenacyMiddleware(RequestDelegate next)
{

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        Log.Information("Invoking Multi-Tenacy Middleware");
        
        var compId = context.User.Claims.FirstOrDefault(c => c.Type == "mf:cid")?.Value;

        if (!Guid.TryParse(compId, out var companyId))
        {
            Log.Information($"Extracted Company Id: {compId}");
            throw new UnauthorizedAccessException("Invalid company id");
        }
        tenantProvider.SetTenantId(companyId);

        await next(context);
    }
}