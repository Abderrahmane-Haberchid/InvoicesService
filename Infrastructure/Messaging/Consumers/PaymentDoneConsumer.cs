using Domain.Enums;
using Domain.Respository;
using InvoicesService.Shared.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers;

public class PaymentDoneConsumer(
    ILogger<PaymentDoneConsumer> logger,
    IInvoiceRepository invoiceRepository) : IConsumer<PaymentDoneEvent>
{
    public async Task Consume(ConsumeContext<PaymentDoneEvent> context)
    {
        logger.LogInformation("=====================================================================");
        logger.LogInformation($"Payment done at {context.Message.PaymentId}, Setting Invoice to PAID...");
        logger.LogInformation("=====================================================================");
        
        var invoice = await invoiceRepository.GetInvoiceByIdAsync(context.Message.InvoiceId, default);

        if (invoice == null)
        {
            throw new KeyNotFoundException("Invoice not found");
        }
        
        invoice.SetStatus(InvoiceStatus.PAID);

        await invoiceRepository.SaveChangeAsync(default);
    }
}