using InvoicesService.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Features.GenerateInvoice;

public class Handler(
    ILogger<Handler> logger) : IConsumer<InvoiceCreatedEvent>
{
    public async Task InvoiceGeneratorAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task Consume(ConsumeContext<InvoiceCreatedEvent> context)
    {
        logger.LogInformation($"Received a new notification at: {DateTime.Now}");
        logger.LogInformation($"Consumed a notification: {context.Message.Id} \n Created At: {context.Message.CreatedAt}");
    }
}