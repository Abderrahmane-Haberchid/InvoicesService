using Application.Abstractions;
using Infrastructure.Caching;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Pdf;
using Infrastructure.Persistance;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddHttpClient();
        
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddScoped<IPdfGenerator, PdfGenerator>();

        services.AddSingleton<ICacheService, HybridCacheService>();
        
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(20),
                LocalCacheExpiration = TimeSpan.FromSeconds(10)
            };
        });

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        
        services.AddMassTransit(busConfiguration =>
        {
            busConfiguration.AddConsumer<PaymentDoneConsumer>();
            
            busConfiguration.AddEntityFrameworkOutbox<AppDbContext>(options =>
            {
                options.UsePostgres();
                options.UseBusOutbox();
                options.DisableInboxCleanupService();
            });
            
            busConfiguration.SetKebabCaseEndpointNameFormatter();
            
            busConfiguration.AddConfigureEndpointsCallback((context, name, cfg) =>
            {
                cfg.UseEntityFrameworkOutbox<AppDbContext>(context);
            });
    
            busConfiguration.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], 
                    ushort.Parse(configuration["RabbitMQ:Port"]!),
                    "/",
                    host =>
                {
                    host.Username(configuration["RabbitMQ:Username"]!);
                    host.Password(configuration["RabbitMQ:Password"]!);
                });
        
                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                cfg.ConfigureEndpoints(context);
            });
        });
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Duende:Authority"];
                options.Audience = configuration["Duende:Audience"];
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true
                };
            });

        services.AddAuthorization();
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =  configuration["Redis:ConnectionStrings"];
        });

        

        return services;
    }
}