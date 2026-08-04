using Domain.Enums;

namespace Application.Features.CreateInvoice.Dtos.requests;

public record InvoiceRequest(
    int CustomerId,
    Guid CompanyId,
    CurrencyType Currency,
    List<InvoiceItemRequest> Items
);

