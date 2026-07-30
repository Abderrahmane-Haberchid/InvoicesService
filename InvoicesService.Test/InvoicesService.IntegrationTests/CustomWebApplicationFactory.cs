using InvoicesService.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    
    private PostgreSqlContainer Container = new PostgreSqlBuilder()
        .WithDatabase("invoices-test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();
    
    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async service =>
        {
            service.RemoveAll<AppDbContext>();
            service.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Container.GetConnectionString());
            });
            
            using var scope = service.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            await dbContext.Database.MigrateAsync();
        });
    }
}