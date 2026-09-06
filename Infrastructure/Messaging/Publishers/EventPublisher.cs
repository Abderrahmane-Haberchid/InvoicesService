using Application.Abstractions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Publishers;

public class EventPublisher(
    IPublishEndpoint publishEndpoint, 
    ILogger<EventPublisher> logger) : IEventPublisher
{
    public async Task PublishAsync<T>(T message,  CancellationToken cancellationToken = default) where T : class
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message), "Cannot publish null event to rabbitMQ Queue");
        
        logger.LogInformation($"Publishing {typeof(T).Name} Event To RabbitMQ Queue");
        
        await publishEndpoint.Publish(message, cancellationToken);
    }
}