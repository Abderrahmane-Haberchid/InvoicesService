
using FluentValidation;
using Application.Features.CreateInvoice;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        
        services.AddScoped<CreateInvoiceValidator>();
        services.AddScoped<InvoiceItemRequestValidator>();
        
        return services;
    }
}