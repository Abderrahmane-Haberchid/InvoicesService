using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Features.AddItem;
using Domain.Enums;
using Domain.Models.Invoice;
using Domain.Respository;
using FluentAssertions;
using InvoicesServiceTest.InvoicesService.IntegrationTests.TestAuth;
using InvoicesServiceTest.InvoicesService.IntegrationTests.WebApplicationFactory;
using Microsoft.Extensions.DependencyInjection;

namespace InvoicesServiceTest.InvoicesService.IntegrationTests;

[Trait("Category", "Integration")]
[Collection("SharedTestCollection")]
public class AddItemIntegrationTest
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
        
        using var scope2 = _factory.Services.CreateScope();
        var tenantProvider2 = scope2.ServiceProvider.GetRequiredService<ITenantProvider>();
        tenantProvider2.SetTenantId(TestClaims.CompanyId);
        var invoiceRepository2 = scope2.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        
        var savedInvoice = await invoiceRepository2.GetInvoiceByIdAsync(invoice.Id, CancellationToken.None);
        
        savedInvoice.Should().NotBeNull();
        savedInvoice!.Items.Should().Contain(i => i.ProductId == command.ProductId);
        
        var body = await result.Content.ReadFromJsonAsync<AddItemResponse>();
        body?.ProductId.Should().Be(command.ProductId);
        body?.InvoiceId.Should().Be(command.InvoiceId);
        body?.Quantity.Should().Be(command.Quantity);
        body?.Total.Should().Be(10 * 120);
    }
    
    [Fact]
    public async Task HandleAsync_ShouldReturn404_WhenInvoiceNotFound()
    {
        await _factory.ResetDatabaseAsync();
        //Arrange
        var http = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
        tenantProvider.SetTenantId(TestClaims.CompanyId);
        
        var invoiceId = Guid.NewGuid();
        var command = new AddItemCommand(invoiceId, 123, 10, 120);
        
        //Act
        var result = await http.PostAsJsonAsync($"api/v1/invoices/{invoiceId}/items", command);
        
        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    } 
    [Fact]
    public async Task HandleAsync_ShouldIncrementItemQuantity_WhenProductAlreadyExist()
    {
        await _factory.ResetDatabaseAsync();
        //Arrange
        var http = _factory.CreateClient();
        
        using var scope = _factory.Services.CreateScope();
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
        tenantProvider.SetTenantId(TestClaims.CompanyId);
        
        var invoice = Invoice.Create(TestClaims.CompanyId, 123, CurrencyType.EUR);
        
        await invoiceRepository.CreateInvoiceAsync(invoice, CancellationToken.None);
        await invoiceRepository.SaveChangeAsync(CancellationToken.None);

        List<AddItemCommand> items =
        [
            new (invoice.Id, 123, 10, 120),
            new (invoice.Id, 123, 10, 120)
        ];

        //Act
          foreach (var item in items)
          {
              await http.PostAsJsonAsync($"api/v1/invoices/{invoice.Id}/items", item);
          }
          
        using var scope2 = _factory.Services.CreateScope();
        var tenantProvider2 = scope2.ServiceProvider.GetRequiredService<ITenantProvider>();
        tenantProvider2.SetTenantId(TestClaims.CompanyId);
        var invoiceRepo2 = scope2.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        var savedInvoice = await invoiceRepo2.GetInvoiceByIdAsync(invoice.Id, CancellationToken.None);
        
        //Assert
        savedInvoice.Should().NotBeNull();
        savedInvoice?.Items.Should().HaveCount(1);
        savedInvoice?.Items.FirstOrDefault()?.Quantity.Should().Be(20);
        savedInvoice?.Items.FirstOrDefault()?.ProductId.Should().Be(123);
        savedInvoice?.Total.Should().Be(2400);
    } 
}