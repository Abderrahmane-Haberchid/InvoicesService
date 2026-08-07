using Infrastructure.Persistance;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

public class InvoiceWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    PostgreSqlContainer postgreSqlContainer;
    RabbitMqContainer rabbitMqContainer;
    RedisContainer redisContainer;
    
    public InvoiceWebApplicationFactory()
    {
        
        postgreSqlContainer = new PostgreSqlBuilder()
            .WithDatabase("Invoices")
            .WithPassword("abdo")
            .WithUsername("abdo")
            .Build();
            
        
        rabbitMqContainer = new RabbitMqBuilder()
            .WithPassword("abdo")
            .WithUsername("abdo")
            .Build();

        redisContainer = new RedisBuilder().Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { "ConnectionStrings:DefaultConnection", postgreSqlContainer.GetConnectionString() },
                    { "ConnectionStrings:Host", rabbitMqContainer.Hostname },
                    { "ConnectionStrings:Port", rabbitMqContainer.GetMappedPublicPort(5672).ToString() },
                    { "ConnectionStrings:Username", "abdo" },
                    { "ConnectionStrings:Password", "abdo" },
                    { "Redis:ConnectionStrings", redisContainer.GetConnectionString() }
                });
        });
    }

    public async Task InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();
        await rabbitMqContainer.StartAsync();
        await redisContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await postgreSqlContainer.DisposeAsync();
        await rabbitMqContainer.DisposeAsync();
        await redisContainer.DisposeAsync();
    }
}