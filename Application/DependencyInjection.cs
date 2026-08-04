using InvoicesService.Features.CreateInvoice.Services;
using InvoicesService.Features.GenerateInvoice.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IInvoiceGenerator, IInvoiceGenerator>();
        
        return services;
    }
}