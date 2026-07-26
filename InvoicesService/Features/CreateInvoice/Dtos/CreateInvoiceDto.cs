using InvoicesService.Enums;

namespace InvoicesService.Features.Dtos;

public record CreateInvoiceDto(
    Guid CustomerId,
    CurrencyType Currency,
    List<InvoiceItemDto> Items
);

public abstract record InvoiceItemDto(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);