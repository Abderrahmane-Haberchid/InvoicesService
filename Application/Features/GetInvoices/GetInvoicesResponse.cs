namespace Application.Features.GetInvoices;
using Domain.Enums;

public record GetInvoicesResponse(
    Guid InvoiceId, 
    Guid CompanyId,
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