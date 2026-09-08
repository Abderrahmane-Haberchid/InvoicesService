using Domain.Common;

namespace Application.Common.DomainEventDispacher;

public interface IDomainEventDispacher
{
    Task DispachAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken);
}