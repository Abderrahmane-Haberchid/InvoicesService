
using Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace InvoicesService.Middleware;

public class MultiTenacyMiddleware(
    RequestDelegate next)
{

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        Log.Information("Invoking Multi-Tenacy Middleware");

        var endpoint = context.GetEndpoint();
        
        Console.WriteLine(endpoint);
        
        var isAuthData = endpoint?.Metadata.GetMetadata<IAuthorizeData>() is not null;

        if (!isAuthData)
        {
            await next(context);
            return;
        }
        
        var compId = context.User.Claims.FirstOrDefault(c => c.Type == "mf:cid")?.Value;
        var usrId = context.User.Claims.FirstOrDefault(c => c.Type == "mf:uid")?.Value;
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

        if (!Guid.TryParse(compId, out var companyId))
        {
            throw new UnauthorizedAccessException("Invalid company id");
        }

        if (!Guid.TryParse(usrId, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user id");
        }

        if (string.IsNullOrEmpty(userEmail) || !userEmail.Contains("@"))
        {
            throw new UnauthorizedAccessException("Invalid user email");
        }
        
        tenantProvider.SetTenantId(companyId);
        tenantProvider.SetUserId(userId);
        tenantProvider.SetUserEmail(userEmail);

        await next(context);
    }
}