using Domain.Enums;
using InvoicesService.Domain.DomainExceptions;

namespace Domain.Models;


public class Invoice
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    
    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items;
    public int CustomerId { get; private set; }
    public CurrencyType Currency { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public InvoiceStatus Status { get; private set; }

    private Invoice(
        Guid Id, 
        Guid CompanyId, 
        int  CustomerId,
        CurrencyType Currency, 
        decimal Total, 
        DateTime CreatedAt, 
        InvoiceStatus Status)
    {
        this.Id = Id;
        this.CompanyId = CompanyId;
        this.CustomerId = CustomerId;
        this.Currency = Currency;
        this.Total = Total;
        this.CreatedAt = CreatedAt;
        this.Status = Status;
    }

    public static Invoice Create(
        Guid companyId, 
        int customerId,
        CurrencyType currency)
    {
        if (string.IsNullOrEmpty(companyId.ToString()) ||
            customerId <= 0 ||
            string.IsNullOrEmpty(currency.ToString()))
            throw new InvalidInvoiceDataException("Invoice Data (CompanyId, CustomerId, Currency) are missing!");

        var invoiceId = Guid.NewGuid();
        
        return new Invoice(invoiceId, companyId, customerId, currency, 0.0m, DateTime.Now, InvoiceStatus.CREATED);
    }

    public void AddInvoiceItem(int productId, int quantity, decimal unitPrice)
    {
        // 1. Rule: if product ID already exist, we increase quantity, otherwise we add it
        var existingItem = _items.SingleOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
            Total = CalculateTotal();
            return;
        }
        
        var invoiceItem = InvoiceItem.Create(Id, this, productId, quantity, unitPrice);
        _items.Add(invoiceItem);
        Total = CalculateTotal();
    }

    private decimal CalculateTotal()
    {
        return _items.Sum(item => item.Quantity * item.UnitPrice);
    }

}