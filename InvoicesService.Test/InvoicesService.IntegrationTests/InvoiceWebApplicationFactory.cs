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
    private readonly PostgreSqlContainer postgreSqlContainer = new PostgreSqlBuilder()
        .WithDatabase("Invoices")
        .WithPassword("abdo")
        .WithUsername("abdo")
        .Build();
            
        
    private readonly RabbitMqContainer rabbitMqContainer = new RabbitMqBuilder()
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();

    private readonly RedisContainer redisContainer = new RedisBuilder().Build();

    
    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            postgreSqlContainer.StartAsync(),
            rabbitMqContainer.StartAsync(),
            redisContainer.StartAsync()
        );
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { "ConnectionStrings:DefaultConnection", postgreSqlContainer.GetConnectionString() },
                    { "RabbitMQ:Host", rabbitMqContainer.Hostname },
                    { "RabbitMQ:Port", rabbitMqContainer.GetMappedPublicPort(5672).ToString() },
                    { "RabbitMQ:Username", "guest" },
                    { "RabbitMQ:Password", "guest" },
                    { "Redis:ConnectionStrings", redisContainer.GetConnectionString() }
                });
        });
    }



    public async Task DisposeAsync()
    {
        await postgreSqlContainer.DisposeAsync();
        await rabbitMqContainer.DisposeAsync();
        await redisContainer.DisposeAsync();
    }
}