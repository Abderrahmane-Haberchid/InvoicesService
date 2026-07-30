using FluentValidation;
using InvoicesService.Enums;

namespace InvoicesService.Features.CreateInvoice.Dtos.requests;

public record InvoiceRequest(
    int CustomerId,
    CurrencyType Currency,
    List<InvoiceItemRequest> Items
);

