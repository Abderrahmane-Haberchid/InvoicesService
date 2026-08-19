using Domain.Enums;

namespace Application.Features.GetInvoiceById;

public record GetInvoiceByIdResponse(
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