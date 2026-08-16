using InvoicesService.Shared.Contracts.Events;
using MassTransit;

namespace InvoicesServiceTest.Consumers;

public class FailingCreatedInvoiceConsumerTest : IConsumer<InvoiceCreatedEvent>
{
    public Task Consume(ConsumeContext<InvoiceCreatedEvent> context)
        => throw new InvalidOperationException("Consumer Failed!");
}