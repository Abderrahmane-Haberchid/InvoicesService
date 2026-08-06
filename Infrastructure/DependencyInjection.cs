using Application.Abstractions;
using Application.Features.GenerateInvoice;
using Infrastructure.Caching;
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

        //services.AddScoped<ICacheService, HybridCacheService>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        
        services.AddMassTransit(busConfiguration =>
        {
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
        
            busConfiguration.AddConsumer<Handler>();
    
            busConfiguration.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], "/", host =>
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
                options.Authority = configuration["Keycloak:Authority"];
                options.Audience = configuration["Keycloak:Audience"];
                options.RequireHttpsMetadata = false;
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
            options.Configuration =  configuration["Redis:Configuration"];
        });

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            };
        });

        return services;
    }
}