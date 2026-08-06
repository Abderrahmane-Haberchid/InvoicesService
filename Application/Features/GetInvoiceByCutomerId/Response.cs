using Domain.Enums;

namespace Application.Features.GetInvoiceByCutomerId;

public record Response(
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