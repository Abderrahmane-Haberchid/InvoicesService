
using Application.Features.CreateInvoice;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR();
        
        services.AddScoped<Validator>();
        services.AddScoped<InvoiceItemRequestValidator>();
        
        return services;
    }
}