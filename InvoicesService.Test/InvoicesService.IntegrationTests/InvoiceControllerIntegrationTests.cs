using System.Net;
using System.Net.Http.Json;
using Application.Features.CreateInvoice;
using Domain.Enums;
using Domain.Respository;
using FluentAssertions;
using InvoicesServiceTest.Consumers;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;


public class InvoiceControllerIntegrationTests : IClassFixture<InvoiceWebApplicationFactory>
{

    private readonly InvoiceWebApplicationFactory _factory;
    private readonly IInvoiceRepository _invoiceRepository;
    
    public InvoiceControllerIntegrationTests(InvoiceWebApplicationFactory factory)
    {
        _factory = factory;
        _invoiceRepository = factory.Services.GetRequiredService<IInvoiceRepository>();
        
    }

    [Fact]
    public async Task CreateInvoice_ShouldReturn201_WhenInvoiceIsSavedAndAllDataIsCorrect()
    {
        await _factory.ResetDatabaseAsync();
        
        var client = _factory.CreateClient();
        var command = new CreateInvoiceCommand(
            1,
            Guid.NewGuid(),
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        var response = await client.PostAsJsonAsync("api/v1/invoices", command);
        
        var savedInvoice = await _invoiceRepository.GetAllInvoicesAsync(1 , 50, CancellationToken.None);
        
        Assert.NotNull(savedInvoice[0]);
        savedInvoice.Should().HaveCount(1);
        savedInvoice[0].CustomerId.Should().Be(command.CustomerId);
        savedInvoice[0].CompanyId.Should().Be(command.CompanyId);
        savedInvoice[0].Currency.Should().Be(command.Currency);
        savedInvoice[0].Items.Should().HaveCount(command.Items.Count);
        
        var expectedTotal = command.Items.Sum(i => i.UnitPrice * i.Quantity);
        savedInvoice[0].Total.Should().Be(expectedTotal);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

    }

    [Fact]
    public async Task CreateInvoice_ShouldReturnCreatedStatusCode_WhenInvoiceRepositoryContainOneRecordAndHeaderLocationEndWithInvoiceId()
    {
        await _factory.ResetDatabaseAsync();
        
        var client = _factory.CreateClient();
        var command = new CreateInvoiceCommand(
            1,
            Guid.NewGuid(),
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        var response = await client.PostAsJsonAsync("api/v1/invoices", command);
        var savedInvoice = await _invoiceRepository.GetAllInvoicesAsync(1, 50, default);
            
        response.Headers?.Location?.ToString().EndsWith(savedInvoice[0].Id.ToString()).Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateInvoice_ShouldInvokeEventPublisher_WhenInvoiceIsCreated()
    {
        await _factory.ResetDatabaseAsync();
        
        using var scope =  _factory.Services.CreateScope();
        var massTransitHarness = scope.ServiceProvider.GetRequiredService<BusTestHarness>();
        
        await massTransitHarness.Start();

        massTransitHarness.Published.Should().NotBeNull();
        
        var client = _factory.CreateClient();
        var command = new CreateInvoiceCommand(
            1,
            Guid.NewGuid(),
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        await client.PostAsJsonAsync("api/v1/invoices", command);
        
        var savedInvoices = await _invoiceRepository.GetAllInvoicesAsync(1, 10, default);
        var invoice = savedInvoices.FirstOrDefault();
        
        CreatedInvoiceConsumerTest.InvoiceId.Should().Be(invoice?.Id);

    }
}