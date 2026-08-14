using System.Net;
using System.Net.Http.Json;
using Application.Features.CreateInvoice;
using Domain.Enums;
using Domain.Respository;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

//[CollectionDefinition("Invoice Integration Tests",  DisableParallelization = true)]
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
    public async Task CreateInvoice_ShouldReturn201_WhenInvoiceIsSaved()
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
        
        var savedInvoice = await _invoiceRepository.GetAllInvoicesAsync(1 , 50, CancellationToken.None);
        
        Assert.NotNull(savedInvoice[0]);
        savedInvoice.Should().HaveCount(1);
        response.Headers?.Location?.ToString().EndsWith(savedInvoice[0].Id.ToString()).Should().BeTrue();
    }
}