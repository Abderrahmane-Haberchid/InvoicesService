using Domain.Enums;
using MediatR;

namespace Application.Features.GetInvoiceById;

public record Response(
    Guid InvoiceId, 
    InvoiceStatus Status,
    decimal TotalAmount,
    CurrencyType Currency,
    DateTime CreatedAt,
    List<InvoiceItemQuery> InvoiceItems
);
    
public record InvoiceItemQuery(
    int ProductId,
    int Quantity,
    decimal unitPrice
);