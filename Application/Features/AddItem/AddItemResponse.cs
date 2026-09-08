namespace Application.Features.AddItem;

public record AddItemResponse(
    Guid InvoiceId,
    int ProductId,
    int Quantity,
    decimal Total,
    DateTime Date);