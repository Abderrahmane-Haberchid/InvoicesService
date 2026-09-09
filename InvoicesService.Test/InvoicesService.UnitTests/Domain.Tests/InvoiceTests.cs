using Domain.DomainExceptions;
using Domain.Enums;
using Domain.Models.Invoice;
using Domain.Models.Invoice.Events;
using FluentAssertions;

namespace InvoicesServiceTest.InvoicesService.UnitTests.Domain.Tests;


[Trait("Category", "Unit")]
public class InvoiceTests
{


    [Fact]
    public async Task CreateInvoice_ShouldReturnCreatedInvoice_WhenDataIsValid()
    {
        var companyId = Guid.NewGuid();
        var invoice = Invoice.Create(companyId, 123, CurrencyType.EUR);
        
        invoice.Should().NotBeNull();
        invoice.CompanyId.Should().Be(companyId);
        invoice.CustomerId.Should().Be(123);
        invoice.Currency.Should().Be(CurrencyType.EUR);
    }

    [Fact]
    public async Task CreateInvoice_ShouldThrowInvalidInvoiceDataDomainException_WhenComapnyIdIsEmpty()
    {
        Assert.Throws<InvalidInvoiceDataDomainException>(() =>
            Invoice.Create(Guid.Empty, 123, CurrencyType.EUR));
        
    }
    
    [Fact]
    public async Task CreateInvoice_ShouldThrowInvalidInvoiceDataDomainException_WhenCustomerIdIsZero()
    {
        Assert.Throws<InvalidInvoiceDataDomainException>(() =>
            Invoice.Create(Guid.NewGuid(), 0, CurrencyType.EUR));
        
    }

    [Fact]
    public async Task CreateInvoice_ShouldAddInvoiceCreatedEvent_WhenInvoiceCreated()
    {
        var invoice =  Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR);
        
        invoice.DomainEvents.Should().HaveCount(1);
        invoice.DomainEvents.Should().Contain(e => e is InvoiceCreatedDomainEvent);
    }
    
    [Fact]
    public async Task AddItemToInvoice_ShouldAddItemToInvoice_WhenInvoiceCreated()
    {
        var invoice = Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR);
        invoice.AddInvoiceItem(1, 10, 320);
        invoice.AddInvoiceItem(2, 10, 320);
        invoice.AddInvoiceItem(3, 10, 320);
        
        decimal total = invoice.Items.Sum(item => item.Quantity * item.UnitPrice);
        
        invoice.Items.Should().HaveCount(3);
        invoice.Items.Should().AllBeOfType<InvoiceItem>();
        invoice.Total.Should().Be(total);
    }
    
    [Fact]
    public async Task AddItemToInvoice_ShouldAddItemAndOnlyIncrementQuantity_WhenItemAlreadyExist()
    {
        var invoice = Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR);
        invoice.AddInvoiceItem(1, 10, 320);
        invoice.AddInvoiceItem(1, 10, 320);
        invoice.AddInvoiceItem(1, 10, 320);
        
        invoice.Items.Should().HaveCount(1);
        invoice.Total.Should().Be(9600);
        invoice.Items.FirstOrDefault(item => item.ProductId == 1)?.Quantity.Should().Be(30);
    }
    
    [Fact]
    public async Task AddItemToInvoice_ShouldRaiseItemAddedEvent_WhenItemAddedToInvoice()
    {
        var invoice = Invoice.Create(Guid.NewGuid(), 123, CurrencyType.EUR);
        invoice.AddInvoiceItem(1, 10, 320);
        invoice.AddInvoiceItem(10, 10, 320);
        invoice.AddInvoiceItem(100, 10, 320);
        
        invoice.DomainEvents.Should().HaveCount(4); // plus invoice created event
        invoice.DomainEvents.Should().Contain(i => i is ItemAddedDomainEvent);
        invoice.DomainEvents.Should().Contain(i => i is InvoiceCreatedDomainEvent);
    }

}