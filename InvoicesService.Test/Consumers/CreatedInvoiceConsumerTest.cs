using InvoicesService.Shared.Contracts.Events;
using MassTransit;

namespace InvoicesServiceTest.Consumers;

public class CreatedInvoiceConsumerTest : IConsumer<InvoiceCreatedEvent>
{
    public static Guid? InvoiceId;
    public Task Consume(ConsumeContext<InvoiceCreatedEvent> context)
    {
        InvoiceId = context.Message.InvoiceId;
        
        return Task.CompletedTask;
    }
}