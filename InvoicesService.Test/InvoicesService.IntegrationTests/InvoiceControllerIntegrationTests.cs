using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Features.CreateInvoice;
using Domain.Enums;
using Domain.Respository;
using FluentAssertions;
using InvoicesService.Shared.Contracts.Events;
using InvoicesServiceTest.InvoicesService.IntegrationTests.TestAuth;
using InvoicesServiceTest.InvoicesService.IntegrationTests.WebApplicationFactory;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;


[Trait("Category", "Integration")]
[Collection("SharedTestCollection")]
public class InvoiceControllerIntegrationTests
{
    
    private readonly InvoiceWebApplicationFactory _factory;
    
    public InvoiceControllerIntegrationTests(InvoiceWebApplicationFactory factory)
    {
        _factory = factory;
        
    }

    [Fact]
    public async Task CreateInvoice_ShouldReturn201_WhenInvoiceIsSavedAndAllDataIsCorrect()
    {
        await _factory.ResetDatabaseAsync();
        
        using var scope = _factory.Services.CreateScope();
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
        tenantProvider.SetTenantId(TestClaims.CompanyId);
        
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        
        var client = _factory.CreateClient();
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        var response = await client.PostAsJsonAsync("api/v1/invoices", command);
        
        var savedInvoices = await invoiceRepository.GetAllInvoicesAsync(1 , 50, CancellationToken.None);
        var savedInvoice = savedInvoices.FirstOrDefault();
        Assert.NotNull(savedInvoice);
        savedInvoice?.CustomerId.Should().Be(command.CustomerId);
        savedInvoice?.Currency.Should().Be(command.Currency);
        savedInvoice?.Items.Should().HaveCount(command.Items.Count);
        
        var expectedTotal = command.Items.Sum(i => i.UnitPrice * i.Quantity);
        savedInvoice?.Total.Should().Be(expectedTotal);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

    }

    [Fact]
    public async Task CreateInvoice_ShouldReturnCreatedStatusCode_WhenInvoiceRepositoryContainOneRecordAndHeaderLocationEndWithInvoiceId()
    {
        await _factory.ResetDatabaseAsync();
        using var scope = _factory.Services.CreateScope();
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
        tenantProvider.SetTenantId(TestClaims.CompanyId);
        
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        
        var client = _factory.CreateClient();
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        var response = await client.PostAsJsonAsync("api/v1/invoices", command);
        var savedInvoice = await invoiceRepository.GetAllInvoicesAsync(1, 50, default);
            
        response.Headers?.Location?.ToString().EndsWith(savedInvoice[0].Id.ToString()).Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateInvoice_ShouldConsumeInvoiceCreatedEvent_WhenInvoiceCreated()
    {
        await _factory.ResetDatabaseAsync();
        using var scope = _factory.Services.CreateScope();
        
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var client = _factory.CreateClient();
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        await client.PostAsJsonAsync("api/v1/invoices", command);
        
        Assert.True(await harness.Consumed.Any<InvoiceCreatedEvent>());
    }

    [Fact]
    public async Task CreateInvoice_ShouldPublishInvoiceCreatedEvent_WhenInvoiceCreated()
    {
        await _factory.ResetDatabaseAsync();
        using var scope = _factory.Services.CreateScope();
        
        var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
        await harness.Start();
        
        var client = _factory.CreateClient();
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        await client.PostAsJsonAsync("api/v1/invoices", command);
        Assert.True(await harness.Published.Any<InvoiceCreatedEvent>());
    }
}