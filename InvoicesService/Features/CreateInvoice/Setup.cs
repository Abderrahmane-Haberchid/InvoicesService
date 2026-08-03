using InvoicesService.Features.CreateInvoice.Services;
using InvoicesService.Features.CreateInvoice.Validators;

namespace InvoicesService.Features.CreateInvoice;

public static class Setup
{
    public static IServiceCollection AddCreateInvoiceServices(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<InvoiceRequestValidator>();
        services.AddScoped<InvoiceItemRequestValidator>();
        
        return services;
    }
}