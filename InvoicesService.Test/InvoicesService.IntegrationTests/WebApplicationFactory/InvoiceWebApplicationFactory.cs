using Infrastructure.Persistance;
using InvoicesServiceTest.Consumers;
using InvoicesServiceTest.InvoicesService.IntegrationTests.TestAuth;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests.WebApplicationFactory;

public class InvoiceWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private Respawner? _respawner;
    private NpgsqlConnection? _dbConnection;
    private SemaphoreSlim _semaphoreSlim = new  (1, 1);

    private readonly PostgreSqlContainer _postgreSqlContainer =
        new PostgreSqlBuilder("postgres:15-alpine")
            .WithDatabase("Invoices")
            .WithUsername("abdo")
            .WithPassword("abdo")
            .Build();

    private readonly RabbitMqContainer _rabbitMqContainer =
        new RabbitMqBuilder("rabbitmq:4-management-alpine")
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();

    private readonly RedisContainer _redisContainer =
        new RedisBuilder("redis:8")
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
        
        await InitializeRespawnerAsync();
        
    }

    private async Task InitializeRespawnerAsync()
    {
        using var scope = Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await database.Database.EnsureDeletedAsync();
        await database.Database.EnsureCreatedAsync();
        
        _dbConnection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());
        _dbConnection.Open();
        
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres
            });
    }

    public async Task ResetDatabaseAsync()
    {
        if (_respawner is null)
            throw new InvalidOperationException();

        CreatedInvoiceConsumerTest.Clear();
        await _semaphoreSlim.WaitAsync();

        try
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    if (dbContext.OutboxMessages.Any())
                    {
                        await Task.Delay(200);
                        continue;
                    }

                    if (_dbConnection != null)
                    {
                        await _respawner.ResetAsync(_dbConnection);
                        return;
                    }
                }
                catch (PostgresException ex) when (ex.SqlState == "40P01")
                {
                    // Deadlock detected, retry
                    await Task.Delay(100);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { });
            
            services.AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<CreatedInvoiceConsumerTest>();
                x.AddConsumer<FailingCreatedInvoiceConsumerTest>();
            });
        });
        
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _postgreSqlContainer.GetConnectionString(),

                    ["RabbitMQ:Host"] = _rabbitMqContainer.Hostname,

                    ["RabbitMQ:Port"] =
                        _rabbitMqContainer
                            .GetMappedPublicPort(5672)
                            .ToString(),

                    ["RabbitMQ:Username"] = "guest",

                    ["RabbitMQ:Password"] = "guest",

                    ["Redis:ConnectionStrings"] = _redisContainer.GetConnectionString()
                });
        });
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgreSqlContainer.DisposeAsync().AsTask(),
            _rabbitMqContainer.DisposeAsync().AsTask(),
            _redisContainer.DisposeAsync().AsTask()
        );
        _dbConnection?.Dispose();
        _semaphoreSlim.Dispose();
    }
}