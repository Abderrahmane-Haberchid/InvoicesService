using Domain.Enums;

namespace Application.Features.GetInvoiceByCustomerId;

public record GetInvoiceByCustomerIdResponse(
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