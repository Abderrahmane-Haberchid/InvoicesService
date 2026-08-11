namespace InvoicesService.Shared.Contracts.Events;

public class PaymentDoneEvent
{
    public Guid PaymentId { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Total { get; set; }
    public DateTime PaidAt { get; set; }
}