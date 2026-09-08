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
        var message = context.Message;
        
        logger.LogInformation("=====================================================================");
        logger.LogInformation("Payment done at {MessagePaymentId}, Setting Invoice to PAID...", message.PaymentId);
        logger.LogInformation("=====================================================================");
        
        var invoice = await invoiceRepository.GetInvoiceByIdAsync(message.InvoiceId, CancellationToken.None);

        if (invoice == null)
        {
            throw new KeyNotFoundException("Invoice not found");
        }
        
        invoice.SetStatus(InvoiceStatus.PAID);

        await invoiceRepository.SaveChangeAsync(CancellationToken.None);
    }
}