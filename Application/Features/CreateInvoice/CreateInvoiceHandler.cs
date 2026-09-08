using Application.Abstractions;
using Application.Common.DomainEventDispacher;
using Application.Common.DomainEventHandlers;
using Application.Exceptions;
using Domain.Models;
using Domain.Models.Invoice;
using Domain.Respository;
using FluentValidation;
using InvoicesService.Shared.Contracts.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.CreateInvoice;

public class CreateInvoiceHandler(
    IInvoiceRepository invoiceRepository,
    IValidator<CreateInvoiceCommand> validator,
    IEventPublisher eventPublisher,
    IDomainEventDispacher domainEventDispacher,
    ITenantProvider tenantProvider,
    ILogger<CreateInvoiceHandler> logger) 
    : IRequestHandler<CreateInvoiceCommand, CreateInvoiceResponse>
{
    public async Task<CreateInvoiceResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var validate = await validator.ValidateAsync(request, cancellationToken);
        if (!validate.IsValid)
        {
            throw new ValidationException(validate.Errors);
        }

        if (tenantProvider.TenantId == Guid.Empty)
        {
            throw new TenantIdMissingException("Company Id could not be null");
        } 
        
        var invoice = Invoice.Create(
            tenantProvider.TenantId,
            request.CustomerId,
            request.Currency);
        
        request.Items
            .ForEach(i => invoice.AddInvoiceItem(i.ProductId, i.Quantity, i.UnitPrice));
        
        var savedInvoice = await invoiceRepository.CreateInvoiceAsync(invoice, cancellationToken);

        if (savedInvoice is null)
            throw new NullObjectReturnedFromCreateRepositoryException("No Invoice Saved !");
        
        await domainEventDispacher.DispachAsync(invoice.DomainEvents, cancellationToken);

        var response = savedInvoice.ToResponse();

        await eventPublisher.PublishAsync(new InvoiceCreatedEvent(
            savedInvoice.Id,
            savedInvoice.CustomerId,
            savedInvoice.Total,
            DateTime.UtcNow), cancellationToken);
        
        await invoiceRepository.SaveChangeAsync(cancellationToken); 

        return response;
    }
}