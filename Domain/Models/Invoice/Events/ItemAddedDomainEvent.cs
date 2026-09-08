using Domain.Common;

namespace Domain.Models.Invoice.Events;

public sealed record ItemAddedDomainEvent(
    Guid InvoiceId,
    int ProductId,
    decimal UnitPrice,
    int Quantity,
    decimal Total) : IDomainEvent

{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}