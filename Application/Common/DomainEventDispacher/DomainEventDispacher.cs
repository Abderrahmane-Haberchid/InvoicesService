using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.DomainEventDispacher;

public class DomainEventDispacher(
    IPublisher publisher,
    ILogger<DomainEventDispacher> logger) : IDomainEventDispacher
{
    public async Task DispachAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing {domainEvents} Domain Events", domainEvents.Count);
        
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}