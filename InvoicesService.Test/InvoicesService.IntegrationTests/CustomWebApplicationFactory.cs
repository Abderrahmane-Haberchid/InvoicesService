using InvoicesService.DbContext;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Org.BouncyCastle.Crypto.Utilities;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    
    private PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithDatabase("invoices-test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();
    
    private RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();
    
    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.Configure<HealthCheckServiceOptions>(options =>
        {
            var masstransitChecks = options.Registrations
                .Where(x => x.Tags.Contains("masstransit"))
                .ToList();

            foreach (var check in masstransitChecks)
            {
                options.Registrations.Remove(check);
            }
        });
        
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });
            
            services.RemoveAll<IBus>();
            services.RemoveAll<IBusControl>();
            services.AddMassTransit(busConfiguration =>
            {
                busConfiguration.ConfigureHealthCheckOptions(options =>
                {
                    options.Name = null;
                });
                busConfiguration.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(_rabbitMqContainer.GetConnectionString());
                });
            });
            
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            dbContext.Database.Migrate();
        });
    }
}