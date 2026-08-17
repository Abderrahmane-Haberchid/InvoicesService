namespace InvoicesServiceTest.Events;

public record InvoiceCreatedFailingEvent(
    Guid InvoiceId,
    int  CustomerId,
    decimal Total,
    DateTime CreatedAt);