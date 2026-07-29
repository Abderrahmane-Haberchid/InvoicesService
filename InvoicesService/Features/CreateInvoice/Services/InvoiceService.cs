using FluentValidation;
using InvoicesService.Enums;
using InvoicesService.Features.CreateInvoice.Dtos;
using InvoicesService.Features.CreateInvoice.Dtos.requests;
using InvoicesService.Models;
using InvoicesService.Respository;

namespace InvoicesService.Features.CreateInvoice.Services;

public class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IValidator<CreateInvoiceDto> validator,
    ILogger<InvoiceService> logger) 
    : IInvoiceService
{
    public async Task<InvoiceResponseDto> CreateAsync(
        CreateInvoiceDto invoiceDto, 
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(invoiceDto, cancellationToken);
        
        if(!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        logger.LogInformation("Invoice for customer {id} start processing in invoice service", 
            invoiceDto.CustomerId);
        
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId,
            CustomerId = invoiceDto.CustomerId,
            Currency = invoiceDto.Currency,
            Total = invoiceDto.Items.Sum(item => item.Quantity * item.UnitPrice),
            CreatedAt = DateTime.UtcNow,
            Status = InvoiceStatus.CREATED,
            Items = ExtractInvoiceItems(invoiceDto, invoiceId)
        };
        
        var savedInvoice = await invoiceRepository
            .CreateInvoiceAsync(invoice, cancellationToken);

        return new InvoiceResponseDto(
            savedInvoice.Id,
            savedInvoice.Status,
            savedInvoice.Total,
            savedInvoice.Currency,
            savedInvoice.CreatedAt  
        );

    }

    private List<InvoiceItems> ExtractInvoiceItems(CreateInvoiceDto invoiceDto, Guid invoiceId)
    {
        return invoiceDto.Items.Select(invoiceItem => new InvoiceItems
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