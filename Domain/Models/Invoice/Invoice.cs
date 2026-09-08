using Domain.Common;
using Domain.DomainExceptions;
using Domain.Enums;
using Domain.Models.Invoice.Events;

namespace Domain.Models.Invoice;


public class Invoice : AggregateRoot
{
    public Guid CompanyId { get; private set; }
    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items;
    public int CustomerId { get; private set; }
    public CurrencyType Currency { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public Guid RowVersion { get; private set; } = Guid.NewGuid();

    private Invoice(
        Guid companyId, 
        int  customerId,
        CurrencyType currency, 
        decimal total, 
        DateTime createdAt, 
        InvoiceStatus status) : base(Guid.NewGuid())
    {
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
        if (companyId == Guid.Empty ||
            customerId <= 0 ||
            !Enum.IsDefined(currency))
        { 
            throw new InvalidInvoiceDataDomainException(
                "Invoice Data (CompanyId, CustomerId, Currency) are missing!");   
        }
        
        var invoice = new Invoice(
            companyId, 
            customerId, 
            currency, 
            0.0m, 
            DateTime.UtcNow,
            InvoiceStatus.CREATED);
        
        invoice.AddDomainEvent(
            new InvoiceCreatedDomainEvent(
                invoice.Id, 
                invoice.CompanyId, 
                invoice.Total));
        
        return invoice;
    }

    public void AddInvoiceItem(int productId, int quantity, decimal unitPrice)
    {
        var existingItem = _items.SingleOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
            Total = CalculateTotal();
            
            AddDomainEvent(new ItemAddedDomainEvent(
                Id,
                existingItem.ProductId,
                existingItem.UnitPrice,
                existingItem.Quantity,
                Total));
            
            UpdateRowVersion();
            return;
        }
        
        var invoiceItem = InvoiceItem.Create(Id, this, productId, quantity, unitPrice);
        _items.Add(invoiceItem);
        Total = CalculateTotal();
        
        AddDomainEvent(new ItemAddedDomainEvent(
            Id,
            productId,
            unitPrice,
            quantity,
            Total));
        UpdateRowVersion();;
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
            throw new InvoiceStatusAlreadyAssignedException($"Invoice already has same status {status}");
        }
        Status = status;
        UpdateRowVersion();;
    }

    private void UpdateRowVersion()
    {
        RowVersion = Guid.NewGuid();
    }
}