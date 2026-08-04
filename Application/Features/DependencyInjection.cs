using Application.Features.CreateInvoice.Services;
using Application.Features.CreateInvoice.Validators;
using InvoicesService.Features.CreateInvoice.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Features;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<InvoiceRequestValidator>();
        services.AddScoped<InvoiceItemRequestValidator>();
        
        return services;
    }
}