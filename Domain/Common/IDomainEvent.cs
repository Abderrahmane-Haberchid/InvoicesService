using MediatR;

namespace Domain.Common;

public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
    public DateTime OccurredOn { get; init; }
}