using InvoicesService.Shared.Enums;

namespace InvoicesService.Features.CreateInvoice.Dtos.responses;

public record InvoiceResponseDto(
    Guid InvoiceId, 
    InvoiceStatus Status,
    decimal TotalAmount,
    CurrencyType Currency,
    DateTime CreatedAt
);