using Domain.Enums;

namespace Application.Features.CreateInvoice.Dtos.responses;

public record InvoiceResponseDto(
    Guid InvoiceId, 
    InvoiceStatus Status,
    decimal TotalAmount,
    CurrencyType Currency,
    DateTime CreatedAt,
    List<InvoiceItemResponse> InvoiceItems
    );