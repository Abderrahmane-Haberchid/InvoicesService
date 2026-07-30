using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using InvoicesService.DbContext;
using InvoicesService.Enums;
using InvoicesService.Features.CreateInvoice.Dtos.requests;
using InvoicesService.Features.CreateInvoice.Dtos.responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

public class EndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private CustomWebApplicationFactory _factory;

    public EndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task ShouldReturnCreated_WhenInvoiceIsCreated()
    {
        var client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var invoice = new InvoiceRequest(
            123,
            CurrencyType.USD,
            new List<InvoiceItemRequest>
            {
                new (1111, 10, 230),
                new (1114, 10, 230),
                new (1113, 10, 230),
                new (1112, 10, 230),
            }
            );
        var response = await client.PostAsJsonAsync("/api/v1/invoices", invoice);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        

        var savedInvoice = await dbContext
            .Invoices
            .Include(item => item.Items)
            .SingleAsync();
        
        savedInvoice.Items.Count.Should().Be(4);
        savedInvoice.CustomerId.Should().Be(invoice.CustomerId);
        savedInvoice.Currency.Should().Be(invoice.Currency);

        savedInvoice.Items.Select(item => new
            {
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
            })
            .Should()
            .BeEquivalentTo(invoice.Items.Select(i =>
                new
                {
                    i.ProductId,
                    i.Quantity,
                    i.UnitPrice
                    
                }));
        
        var expectedTotal = invoice.Items.Sum(item => item.Quantity * item.UnitPrice);
        
        savedInvoice.Total.Should().Be(expectedTotal);
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new JsonStringEnumConverter());

        var body = await response.Content.ReadFromJsonAsync<InvoiceResponseDto>(options);
        
        body.Should().NotBeNull();
        body.Currency.Should().Be(invoice.Currency);
        body.TotalAmount.Should().Be(expectedTotal);
        
        response?.Headers?.Location?.ToString().Should().EndWith($"api/v1/invoices/{body.InvoiceId}");
    }
    
    [Fact]
    public async Task ShouldReturnBadRequest_WhenCustomerIdIsZero()
    {
        var client = _factory.CreateClient();
        
        var invoice = new InvoiceRequest(
            0,
            CurrencyType.USD,
            new List<InvoiceItemRequest>
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
        var client = _factory.CreateClient();
        var invoice = new InvoiceRequest(
            0,
            CurrencyType.USD,
            new List<InvoiceItemRequest>()
        );
        
        var response = await client.PostAsJsonAsync("/api/v1/invoices", invoice);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenInvoiceIsNull()
    {
        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync("/api/v1/invoices", (InvoiceRequest?)null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    } 
}