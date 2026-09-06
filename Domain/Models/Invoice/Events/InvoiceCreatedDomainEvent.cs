using Domain.Common;

namespace Domain.Models.Invoice.Events;

public sealed record InvoiceCreatedDomainEvent(
    Guid InvoiceId,
    Guid CompanyId,
    decimal Total
    ) : IDomainEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
}