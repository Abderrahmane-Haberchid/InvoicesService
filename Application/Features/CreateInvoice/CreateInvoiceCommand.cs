using Domain.Enums;
using MediatR;

namespace Application.Features.CreateInvoice;

public sealed record CreateInvoiceCommand(
    int CustomerId,
    CurrencyType Currency,
    List<InvoiceItemCommand> Items
) : IRequest<CreateInvoiceResponse>;

public sealed record InvoiceItemCommand(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);

