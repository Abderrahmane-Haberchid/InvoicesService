using Domain.Enums;
using MediatR;

namespace Application.Features.CreateInvoice;

public record CreateInvoiceCommand(
    int CustomerId,
    CurrencyType Currency,
    List<InvoiceItemCommand> Items
) : IRequest<CreateInvoiceResponse>;

public record InvoiceItemCommand(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);

