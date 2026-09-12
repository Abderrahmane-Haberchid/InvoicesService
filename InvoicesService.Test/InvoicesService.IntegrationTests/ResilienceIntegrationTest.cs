using System.Net;
using System.Net.Http.Json;
using Application.Features.CreateInvoice;
using Domain.Enums;
using FluentAssertions;
using InvoicesServiceTest.InvoicesService.IntegrationTests.WebApplicationFactory;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

[Collection("SharedTestCollection")]
[Trait("Category", "Integration")]
public class ResilienceIntegrationTest
{
    private InvoiceWebApplicationFactory _factory;
    
    public ResilienceIntegrationTest(InvoiceWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateInvoice_ShouldReturn429_WhenTokenBurstIsExceeded()
    {
        //Arrange
        var client = _factory.CreateClient();
        var command = new CreateInvoiceCommand(
            1,
            CurrencyType.USD,
            [
                new InvoiceItemCommand(1, 10, 400),
                new InvoiceItemCommand(2, 10, 400),
                new InvoiceItemCommand(3, 10, 400)
            ]);
        
        //Act
        var request = Enumerable
            .Range(1, 400)
            .Select(_ => client.PostAsJsonAsync("api/v1/invoices", command));
        
        var result = await Task.WhenAll(request);
        
        //Assert
        result.Should().Contain(r => r.StatusCode == HttpStatusCode.TooManyRequests);
        result.Should().Contain(r => r.StatusCode == HttpStatusCode.Created);
    }
}