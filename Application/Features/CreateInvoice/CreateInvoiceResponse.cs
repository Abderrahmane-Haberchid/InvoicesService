using Domain.Enums;

namespace Application.Features.CreateInvoice;

public record CreateInvoiceResponse(
    Guid InvoiceId, 
    InvoiceStatus Status,
    decimal TotalAmount,
    CurrencyType Currency,
    DateTime CreatedAt,
    List<ItemResponse> InvoiceItems
    );
    
public record ItemResponse(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);