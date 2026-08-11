using InvoicesService.Shared.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers;

public class PaymentDoneConsumer(ILogger<PaymentDoneConsumer> logger) : IConsumer<PaymentDoneEvent>
{
    public Task Consume(ConsumeContext<PaymentDoneEvent> context)
    {
        logger.LogInformation("=====================================================================");
        logger.LogInformation($"Payment done at {context.Message.PaymentId}");
        logger.LogInformation($"Invoice id {context.Message.InvoiceId}");
        logger.LogInformation($"Payment done at {context.Message.Total}");
        logger.LogInformation($"Payment done at {context.Message.PaidAt}");
        logger.LogInformation("=====================================================================");
        
        return Task.CompletedTask;
    }
}