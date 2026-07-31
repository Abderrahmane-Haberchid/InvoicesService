using System.Data;
using FluentValidation;
using InvoicesService.Enums;
using InvoicesService.Features.CreateInvoice.Dtos.requests;
using InvoicesService.Features.CreateInvoice.Dtos.responses;
using InvoicesService.Features.CreateInvoice.Validators;
using InvoicesService.Models;
using InvoicesService.Respository;
using InvoicesService.Shared.Contracts;
using MassTransit;
using Npgsql;

namespace InvoicesService.Features.CreateInvoice.Services;

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
        
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            CustomerId = invoiceRequest.CustomerId,
            Currency = invoiceRequest.Currency,
            Total = invoiceRequest.Items.Sum(item => item.Quantity * item.UnitPrice),
            CreatedAt = DateTime.UtcNow,
            Status = InvoiceStatus.CREATED,
            Items = ExtractInvoiceItems(invoiceRequest, invoiceId)
        };
        
        var savedInvoice = await invoiceRepository.CreateInvoiceAsync(invoice, cancellationToken);

        if (savedInvoice is null)
            throw new DataException();

        await publishEndpoint.Publish(new InvoiceCreatedEvent(
                savedInvoice.Id,
                savedInvoice.CustomerId,
                savedInvoice.Total,
                DateTime.UtcNow
            ), cancellationToken);

        return new InvoiceResponseDto(
            savedInvoice.Id,
            savedInvoice.Status,
            savedInvoice.Total,
            savedInvoice.Currency,
            savedInvoice.CreatedAt  
        );

    }

    private List<InvoiceItems> ExtractInvoiceItems(InvoiceRequest invoiceRequest, Guid invoiceId)
    {
        return invoiceRequest.Items.Select(invoiceItem => new InvoiceItems
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoiceId,
                ProductId = invoiceItem.ProductId,
                Quantity = invoiceItem.Quantity,
                UnitPrice = invoiceItem.UnitPrice,
            })
            .ToList();
    }
}