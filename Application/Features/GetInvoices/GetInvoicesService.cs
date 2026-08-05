using Application.Features.CreateInvoice.Dtos.responses;
using Domain.Respository;
using Microsoft.Extensions.Logging;

namespace Application.Features.GetInvoices;

public class GetInvoicesService(
    IInvoiceRepository invoiceRepository,
    ILogger<GetInvoicesService> logger) : IGetInvoicesService
{
    public async Task<List<InvoiceResponseDto>> GetInvoicesByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        if (customerId <= 0)
        {
            throw new ArgumentNullException(nameof(customerId), "CustomerId Should not be Empty");
        }

        var invoices = await invoiceRepository.GetInvoiceByCustomerIdAsync(customerId, cancellationToken);

        if (invoices.Count == 0)
        {
            throw new KeyNotFoundException("Invoices Not Found");
        }

        return invoices.Select(i => new InvoiceResponseDto(
                i.Id,
                i.Status,
                i.Total,
                i.Currency,
                i.CreatedAt,
                i.GetItems()
                    .Select(ii => new InvoiceItemResponse(ii.ProductId, ii.Quantity, ii.UnitPrice))
                    .ToList()))
            .ToList();
    }

    public async Task<InvoiceResponseDto> GetInvoiceByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        if (invoiceId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(invoiceId), "Invoice Id should not be Empty");
        }

        var invoice = await invoiceRepository.GetInvoiceByIdAsync(invoiceId, cancellationToken);

        if (invoice is null)
        {
            throw new KeyNotFoundException($"No invoice found with Id :  '{invoiceId}'");
        }
        logger.LogInformation($"========================================================================");
        logger.LogInformation($"This invoice contain: {invoice.GetItems().Count} Items" );
        logger.LogInformation($"========================================================================");
        return new InvoiceResponseDto(
            invoice.Id, 
            invoice.Status, 
            invoice.Total, 
            invoice.Currency, 
            invoice.CreatedAt, 
            invoice.GetItems().Select(i => new InvoiceItemResponse(i.ProductId, i.Quantity, i.UnitPrice)).ToList());
    }

    public async Task<List<InvoiceResponseDto>> GetAllInvoicesAsync(int take, int skip, CancellationToken cancellationToken)
    {
        if (take <= 0 || skip <= 0)
        {
            throw new ArgumentNullException(nameof(take), "Take or Skip should not be less than 0");
        }

        var invoices = await invoiceRepository.GetAllInvoicesAsync(take, skip, cancellationToken);

        return invoices.Select(i => new InvoiceResponseDto(
            i.Id,
            i.Status,
            i.Total,
            i.Currency,
            i.CreatedAt,
            i.GetItems()
                .Select(ii => new InvoiceItemResponse(ii.ProductId, ii.Quantity, ii.UnitPrice))
                .ToList())
            ).ToList();
    }
}