namespace InvoicesService.Middleware;

public class MultiTenacyMiddleware
{
    private readonly RequestDelegate _next;
    public MultiTenacyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var companyId = context.Request.Headers["companyId"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(companyId))
        {
            throw new InvalidOperationException("CompanyId header is missing");
        }
        
    }
}