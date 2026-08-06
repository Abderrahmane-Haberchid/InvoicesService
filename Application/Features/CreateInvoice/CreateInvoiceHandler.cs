using Application.Exceptions;
using Domain.Models;
using Domain.Respository;
using FluentValidation;
using InvoicesService.Shared.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.CreateInvoice;

public class CreateInvoiceHandler(
    IInvoiceRepository invoiceRepository,
    IValidator<CreateInvoiceCommand> validator,
    IPublishEndpoint publishEndpoint,
    ILogger<CreateInvoiceHandler> logger) 
    : IRequestHandler<CreateInvoiceCommand, CreateInvoiceResponse>
{

    public async Task<CreateInvoiceResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        
        logger.LogInformation("Invoice for customer {id} start processing in invoice service", 
            request.CustomerId);
        
        
        var invoice = Invoice.Create(
            request.CompanyId,
            request.CustomerId,
            request.Currency);
        
        request.Items
            .ForEach(i => invoice.AddInvoiceItem(i.ProductId, i.Quantity, i.UnitPrice));
        
        var savedInvoice = await invoiceRepository.CreateInvoiceAsync(invoice, cancellationToken);

        if (savedInvoice is null)
            throw new NullObjectReturnedFromCreateRepositoryException("No Invoice Saved !");

        await publishEndpoint.Publish(new InvoiceCreatedEvent(
            savedInvoice.Id,
            savedInvoice.CustomerId,
            savedInvoice.Total,
            DateTime.UtcNow), cancellationToken);
        
        await invoiceRepository.SaveChangeAsync(cancellationToken);

        return savedInvoice.ToResponse();
    }
}