using Application.Common.DomainEventDispacher;
using Application.Common.DomainEventHandlers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<InvoiceCreatedEventHandler>();
        services.AddScoped<IDomainEventDispacher, DomainEventDispacher>();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        return services;
    }
}