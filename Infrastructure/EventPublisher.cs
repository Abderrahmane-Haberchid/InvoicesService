using Application.Abstractions;
using MassTransit;

namespace Infrastructure;

public class EventPublisher(IPublishEndpoint publishEndpoint) : IEventPublisher
{
    public async Task PublishAsync<InvoiceCreatedEvent>(InvoiceCreatedEvent message,  CancellationToken cancellationToken = default)
    {
        await publishEndpoint.Publish(message, cancellationToken);
    }
}