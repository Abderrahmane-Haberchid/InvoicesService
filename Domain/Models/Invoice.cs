using Domain.DomainExceptions;
using Domain.Enums;
using Domain.Tenant;

namespace Domain.Models;


public class Invoice :  ITenant
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; set; }
    
    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items;
    public int CustomerId { get; private set; }
    public CurrencyType Currency { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public InvoiceStatus Status { get; private set; }

    private Invoice(
        Guid id, 
        Guid companyId, 
        int  customerId,
        CurrencyType currency, 
        decimal total, 
        DateTime createdAt, 
        InvoiceStatus status)
    {
        Id = id;
        CompanyId = companyId;
        CustomerId = customerId;
        Currency = currency;
        Total = total;
        CreatedAt = createdAt;
        Status = status;
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
        
        return new Invoice(invoiceId, companyId, customerId, currency, 0.0m, DateTime.UtcNow, InvoiceStatus.CREATED);
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
    
    public InvoiceItem? GetItem(int itemId)
    {
        return _items.FirstOrDefault(i => i.ProductId == itemId);
    }

    public IReadOnlyCollection<InvoiceItem> GetItems()
    {
        return _items;
    }

    public void SetStatus(InvoiceStatus status)
    {
        if (Status == status)
        {
            throw new InvoiceStatusAlreadyAssignedException($"Invoice already has an assigned status {status}");
        }
        Status = status;
    }
}