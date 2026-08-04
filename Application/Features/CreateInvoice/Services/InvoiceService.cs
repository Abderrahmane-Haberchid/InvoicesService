using System.Data;
using Application.Features.CreateInvoice.Dtos.requests;
using Application.Features.CreateInvoice.Dtos.responses;
using Application.Features.CreateInvoice.Validators;
using Domain.Models;
using Domain.Respository;
using FluentValidation;
using InvoicesService.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Features.CreateInvoice.Services;

public class InvoiceService(
    IInvoiceRepository invoiceRepository,
    InvoiceRequestValidator invoiceRequestValidator,
    IPublishEndpoint publishEndpoint,
    ILogger<InvoiceService> logger) 
    : IInvoiceService
{
    public async Task<InvoiceResponseDto> CreateAsync(
        InvoiceRequest invoiceRequest,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(invoiceRequest);
        
        await invoiceRequestValidator.ValidateAndThrowAsync(invoiceRequest, cancellationToken);
        
        logger.LogInformation("Invoice for customer {id} start processing in invoice service", 
            invoiceRequest.CustomerId);
        
        
        var invoice = Invoice.Create(
            invoiceRequest.CompanyId,
            invoiceRequest.CustomerId,
            invoiceRequest.Currency);
        
        invoiceRequest.Items
            .ForEach(i => invoice.AddInvoiceItem(i.ProductId, i.Quantity, i.UnitPrice));
        
        var savedInvoice = await invoiceRepository.CreateInvoiceAsync(invoice, cancellationToken);

        if (savedInvoice is null)
            throw new DataException();

        await publishEndpoint.Publish(new InvoiceCreatedEvent(
                savedInvoice.Id,
                savedInvoice.CustomerId,
                savedInvoice.Total,
                DateTime.UtcNow), cancellationToken);
        
        await invoiceRepository.SaveChangeAsync(cancellationToken);

        return new InvoiceResponseDto(
            savedInvoice.Id,
            savedInvoice.Status,
            savedInvoice.Total,
            savedInvoice.Currency,
            savedInvoice.CreatedAt  
        );

    }
}