using InvoicesService.Enums;

namespace InvoicesService.Features.CreateInvoice.Dtos;

public record InvoiceResponseDto(
    Guid InvoiceId, 
    InvoiceStatus Status,
    decimal TotalAmount,
    CurrencyType Currency,
    DateTime CreatedAt
);