
using Infrastructure.Persistance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

public class InvoiceWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private Respawner? _respawner;

    private readonly PostgreSqlContainer _postgreSqlContainer =
        new PostgreSqlBuilder()
            .WithDatabase("Invoices")
            .WithUsername("abdo")
            .WithPassword("abdo")
            .Build();

    private readonly RabbitMqContainer _rabbitMqContainer =
        new RabbitMqBuilder()
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();

    private readonly RedisContainer _redisContainer =
        new RedisBuilder()
            .Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _postgreSqlContainer.StartAsync(),
            _rabbitMqContainer.StartAsync(),
            _redisContainer.StartAsync()
        );

        // Force the ASP.NET test host to be created.
        _ = Services;

        await ApplyMigrationsAsync();
        await InitializeRespawnerAsync();
    }

    private async Task ApplyMigrationsAsync()
    {
        using var scope = Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    private async Task InitializeRespawnerAsync()
    {
        _respawner = await Respawner.CreateAsync(
            _postgreSqlContainer.GetConnectionString(),
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres
            });
    }

    public async Task ResetDatabaseAsync()
    {
        if (_respawner is null)
            throw new InvalidOperationException("Respawner has not been initialized.");

        await _respawner.ResetAsync(_postgreSqlContainer.GetConnectionString());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        _postgreSqlContainer.GetConnectionString(),

                    ["RabbitMQ:Host"] =
                        _rabbitMqContainer.Hostname,

                    ["RabbitMQ:Port"] =
                        _rabbitMqContainer
                            .GetMappedPublicPort(5672)
                            .ToString(),

                    ["RabbitMQ:Username"] = "guest",

                    ["RabbitMQ:Password"] = "guest",

                    ["Redis:ConnectionStrings"] =
                        _redisContainer.GetConnectionString()
                });
        });
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgreSqlContainer.DisposeAsync().AsTask(),
            _rabbitMqContainer.DisposeAsync().AsTask(),
            _redisContainer.DisposeAsync().AsTask()
        );

        await DisposeAsyncCore();
    }

    private async Task DisposeAsyncCore()
    {
        await base.DisposeAsync();
    }
}