namespace Application.Features.CreateInvoice.Dtos.requests;

public record InvoiceItemRequest(
    int ProductId,
    int Quantity,
    decimal UnitPrice
);