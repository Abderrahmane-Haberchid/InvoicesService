namespace InvoicesService.Shared.Events;

public record InvoiceCreatedEvent(
    Guid Id,
    int  CustomerId,
    decimal Total,
    DateTime CreatedAt
    );