using Application.Features.CreateInvoice;
using Domain.Respository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.GetInvoices;

public class Handler(
    IInvoiceRepository invoiceRepository,
    ILogger<Handler> logger) : IRequestHandler<int, List<Response>>
{

    public async Task<List<Response>> GetAllInvoicesAsync(int? page, int? pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0 || pageSize <= 0)
        {
            throw new ArgumentNullException(nameof(page), "Take or Skip should not be less than 0");
        }

        var invoices = await invoiceRepository.GetAllInvoicesAsync(page, pageSize, cancellationToken);

        return invoices.Select(i => new Response(
            i.Id,
            i.Status,
            i.Total,
            i.Currency,
            i.CreatedAt,
            i.GetItems()
                .Select(ii => new InvoiceItemQuery(ii.ProductId, ii.Quantity, ii.UnitPrice))
                .ToList())
            ).ToList();
    }

    public async Task<List<Response>> Handle(int customerId, CancellationToken cancellationToken)
    {
        
    }
}