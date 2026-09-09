using System.Net.Http.Json;
using Application.Abstractions;
using Application.Features.AddItem;
using Application.Features.CreateInvoice;
using Domain.Enums;
using Domain.Models.Invoice;
using Domain.Respository;
using FluentAssertions;
using InvoicesServiceTest.InvoicesService.IntegrationTests.TestAuth;
using Microsoft.Extensions.DependencyInjection;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

public class AddItemIntegrationTest : IClassFixture<InvoiceWebApplicationFactory>
{
    private readonly InvoiceWebApplicationFactory _factory;
    public AddItemIntegrationTest(InvoiceWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn200_WhenItemIsAdded()
    {
        //arrange
        await _factory.ResetDatabaseAsync();
        var http = _factory.CreateClient();
        
        using var scope = _factory.Services.CreateScope();
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
        tenantProvider.SetTenantId(TestClaims.CompanyId);
        
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        
        var companyId = TestClaims.CompanyId;

        var invoice = Invoice.Create(companyId,  123, CurrencyType.EUR);
        await invoiceRepository.CreateInvoiceAsync(invoice, CancellationToken.None);
        await invoiceRepository.SaveChangeAsync(CancellationToken.None);
        
        var command = new AddItemCommand(invoice.Id, 123, 10, 120);

        //Act
        var result = await http.PostAsJsonAsync($"api/v1/invoices/{invoice.Id}/items", command);
        
        //Assert
        result.EnsureSuccessStatusCode();
        var savedInvoice = await invoiceRepository.GetInvoiceByIdAsync(invoice.Id, CancellationToken.None);
        
        savedInvoice.Should().NotBeNull();
        savedInvoice!.Items.Should().Contain(i => i.ProductId == command.ProductId);
        
        var body = await result.Content.ReadFromJsonAsync<AddItemResponse>();
        body?.ProductId.Should().Be(command.ProductId);
        body?.InvoiceId.Should().Be(command.InvoiceId);
        body?.Quantity.Should().Be(command.Quantity);
        body?.Total.Should().Be(10 * 120);
    }
    
    
}