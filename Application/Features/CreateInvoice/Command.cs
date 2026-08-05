using Domain.Enums;
using MediatR;

namespace Application.Features.CreateInvoice;

public record Command(
    int CustomerId,
    Guid CompanyId,
    CurrencyType Currency,
    List<InvoiceItemCommand> Items
) : IRequest<Response>;

public record InvoiceItemCommand(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);

