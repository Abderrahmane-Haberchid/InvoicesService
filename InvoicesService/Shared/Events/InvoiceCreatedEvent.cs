namespace InvoicesService.Shared.Contracts;

public record InvoiceCreatedEvent(
    Guid Id,
    int  CustomerId,
    decimal Total,
    DateTime CreatedAt
    );