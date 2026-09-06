using Domain.Common;

namespace Domain.Models.Invoice.Events;

public record InvoiceCreatedDomainEvent : IDomainEvent
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid CompanyId { get; set; }
    public decimal Total { get; set; }
}