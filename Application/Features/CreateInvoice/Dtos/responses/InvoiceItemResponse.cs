namespace Application.Features.CreateInvoice.Dtos.responses;

public record InvoiceItemResponse(
    int ProductId,
    int Quantity,
    decimal unitPrice
    );