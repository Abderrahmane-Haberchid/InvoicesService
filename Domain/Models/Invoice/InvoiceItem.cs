using Domain.Common;
using Domain.DomainExceptions;

namespace Domain.Models.Invoice;

public class InvoiceItem : Entity
{
    public Guid InvoiceId { get; private set; }
    public Invoice Invoice { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    
    private InvoiceItem() : base(Guid.NewGuid()){}

    internal InvoiceItem(
        Guid invoiceId,
        Invoice invoice, 
        int productId, 
        int quantity, 
        decimal unitPrice) : base(Guid.NewGuid())
    {
        InvoiceId = invoiceId;
        Invoice = invoice;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public static InvoiceItem Create(Guid invoiceId, Invoice invoice, int productId, int quantity, decimal unitPrice)
    {
        if(productId <= 0 || quantity <= 0 || unitPrice <= 0)
            throw new InvalidInvoiceItemDataException("Product and quantity are missing!");
        
        return new InvoiceItem(invoiceId, invoice, productId, quantity, unitPrice);
    }

    public void IncreaseQuantity(int quantity)
    {
        if(quantity <= 0)
            throw new InvalidInvoiceItemDataException("Quantity must be greater than zero!");
        
        Quantity += quantity;
    }
}