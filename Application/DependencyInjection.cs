using System.Net.Security;
using Application.Features.CreateInvoice.Services;
using Application.Features.CreateInvoice.Validators;
using Application.Features.GenerateInvoice.Services;
using InvoicesService.Features.CreateInvoice.Validators;
using InvoicesService.Features.GenerateInvoice.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IInvoiceGenerator, InvoiceGenerator>();
        services.AddScoped<InvoiceRequestValidator>();
        services.AddScoped<InvoiceItemRequestValidator>();
        
        return services;
    }
}