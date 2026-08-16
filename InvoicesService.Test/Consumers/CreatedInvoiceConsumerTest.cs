using InvoicesService.Shared.Contracts.Events;
using MassTransit;

namespace InvoicesServiceTest.Consumers;

public class CreatedInvoiceConsumerTest : IConsumer<InvoiceCreatedEvent>
{
    public static Guid? InvoiceId { get; private set; }
    public static HashSet<int> CustomerIds { get; set; } = new ();
    public Task Consume(ConsumeContext<InvoiceCreatedEvent> context)
    {
        if (!CustomerIds.Add(context.Message.CustomerId))
        {
            return Task.CompletedTask;
        }

        InvoiceId = context.Message.InvoiceId;
        return Task.CompletedTask;
    }
}