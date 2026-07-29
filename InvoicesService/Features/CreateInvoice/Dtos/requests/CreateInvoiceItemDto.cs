namespace InvoicesService.Features.CreateInvoice.Dtos.requests;

public record CreateInvoiceItemDto(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);