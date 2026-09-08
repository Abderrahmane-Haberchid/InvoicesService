using Domain.Models.Invoice.Events;
using MediatR;

namespace Application.Common.DomainEventHandlers;

public class InvoiceCreatedEventHandler : INotificationHandler<InvoiceCreatedDomainEvent>
{
    public Task Handle(
        InvoiceCreatedDomainEvent notification, 
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Notification received from domain {0}", notification);
        return Task.CompletedTask;
    }
}