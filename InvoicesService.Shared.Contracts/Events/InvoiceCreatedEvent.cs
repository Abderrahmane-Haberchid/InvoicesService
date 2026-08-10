namespace InvoicesService.Shared.Contracts.Events;

public record InvoiceCreatedEvent(
    Guid InvoiceId,
    int  CustomerId,
    decimal Total,
    DateTime CreatedAt
    );