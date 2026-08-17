using InvoicesService.Shared.Contracts.Events;
using InvoicesServiceTest.Events;
using MassTransit;

namespace InvoicesServiceTest.Consumers;

public class FailingCreatedInvoiceConsumerTest : IConsumer<InvoiceCreatedFailingEvent>
{
    public Task Consume(ConsumeContext<InvoiceCreatedFailingEvent> context)
        => throw new InvalidOperationException("Consumer Failed!");
}