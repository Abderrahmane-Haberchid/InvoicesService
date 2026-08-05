using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Features.CreateInvoice;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistance;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

public class InvoiceControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private CustomWebApplicationFactory _factory;

    public InvoiceControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ShouldReturnCreatedStatus_WhenOutboxMessageTableHaveOneRecord()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        
        var invoice = new Command(
            123,
            Guid.NewGuid(),
            CurrencyType.USD,
            [
                new(1111, 10, 230),
                new(1114, 10, 230),
                new(1113, 10, 230),
                new(1112, 10, 230)
            ]
        );
        
        await client.PostAsJsonAsync("/api/v1/invoices", invoice);
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var outboxMessages =
            await dbContext.Set<OutboxMessage>()
                .ToListAsync();

        outboxMessages.Should().HaveCount(1);
    }
    
    [Fact]
    public async Task ShouldReturnCreatedStatus_WhenReturnedObjectIsNotNull()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        
        var invoice = new Command(
            123,
            Guid.NewGuid(),
            CurrencyType.USD,
            new List<InvoiceItemCommand>
            {
                new (1111, 10, 230),
                new (1114, 10, 230),
                new (1113, 10, 230),
                new (1112, 10, 230),
            }
            );
        var response = await client.PostAsJsonAsync("/api/v1/invoices", invoice);
        
        Assert.NotNull(response);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task ShouldReturnCreatedStatus_WhenInvoiceDataAreEquivalentToSavedInvoice()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var invoiceRequest = new Command(
            123,
            Guid.NewGuid(),
            CurrencyType.USD,
            new List<InvoiceItemCommand>
            {
                new (1111, 10, 230),
                new (1114, 10, 230),
                new (1113, 10, 230),
                new (1112, 10, 230),
            }
        );
        await client.PostAsJsonAsync("/api/v1/invoices", invoiceRequest);
        
        var savedInvoice = await dbContext
            .Invoices
            .Include(item => item.Items)
            .SingleAsync();
        
        savedInvoice.Items.Count.Should().Be(4);
        savedInvoice.CustomerId.Should().Be(invoiceRequest.CustomerId);
        savedInvoice.Currency.Should().Be(invoiceRequest.Currency);

        savedInvoice.Items.Select(item => new
            {
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
            })
            .Should()
            .BeEquivalentTo(invoiceRequest.Items.Select(i =>
                new
                {
                    i.ProductId,
                    i.Quantity,
                    i.UnitPrice
                    
                }));
        
        var expectedTotal = invoiceRequest.Items.Sum(item => item.Quantity * item.UnitPrice);
        
        savedInvoice.Total.Should().Be(expectedTotal);
    }
    [Fact]
    public async Task ShouldReturnCreatedStatus_WhenInvoiceDataAreEquivalentToRetunedObject()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        
        var invoiceRequest = new Command(
            123,
            Guid.NewGuid(),
            CurrencyType.USD,
            new List<InvoiceItemCommand>
            {
                new (1111, 10, 230),
                new (1114, 10, 230),
                new (1113, 10, 230),
                new (1112, 10, 230),
            }
        );
        var response = await client.PostAsJsonAsync("/api/v1/invoices", invoiceRequest);
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new JsonStringEnumConverter());

        var body = await response.Content.ReadFromJsonAsync<Response>(options);
        
        Assert.NotNull(body);
        body.Currency.Should().Be(invoiceRequest.Currency);
        
        var expectedTotal = invoiceRequest.Items.Sum(item => item.Quantity * item.UnitPrice);
        body.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact]
    public async Task ShouldReturnCreatedStatus_WhenResponseHttpHeaderLocationEndWithInvoiceLink()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        var invoiceRequest = new Command(
            123,
            Guid.NewGuid(),
            CurrencyType.USD,
            new List<InvoiceItemCommand>
            {
                new (1111, 10, 230),
                new (1114, 10, 230),
                new (1113, 10, 230),
                new (1112, 10, 230),
            }
        );
        
        var response = await client.PostAsJsonAsync("/api/v1/invoices", invoiceRequest);
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new JsonStringEnumConverter());
        
        var body = await response.Content.ReadFromJsonAsync<Response>(options);
        Assert.NotNull(body);
        response?.Headers?.Location?.ToString().Should().EndWith($"api/v1/invoices/{body.InvoiceId}");
    }
    
    [Fact]
    public async Task ShouldReturnBadRequest_WhenCustomerIdIsZero()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        
        var invoice = new Command(
            0,
            Guid.NewGuid(),
            CurrencyType.USD,
            new List<InvoiceItemCommand>
            {
                new (1111, 10, 230),
                new (1114, 10, 230),
                new (1113, 10, 230),
                new (1112, 10, 230),
            }
        );
        
        var response = await client.PostAsJsonAsync("/api/v1/invoices", invoice);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenInvoiceItemsIsEmpty()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        var invoice = new Command(
            0,
            Guid.NewGuid(),
            CurrencyType.USD,
            []
        );
        
        var response = await client.PostAsJsonAsync("/api/v1/invoices", invoice);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenInvoiceIsNull()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync("/api/v1/invoices", (Command?)null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    } 
}