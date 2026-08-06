
using Domain.Respository;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Features.GetInvoices;

public class Handler(
    IInvoiceRepository invoiceRepository,
    HybridCache  hybridCache,
    ILogger<Handler> logger) : IRequestHandler<Query, List<Response>>
{

    public async Task<List<Response>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
        {
            throw new ArgumentNullException(nameof(request), "Page and PageSize should not be less than 0");
        }

        var invoices = await hybridCache.GetOrCreateAsync(
            $"invoices:{Guid.NewGuid()}",
            async ct =>
            {
                Console.WriteLine("🔥 Database hit");
                
                return await invoiceRepository.GetAllInvoicesAsync(request.Page, request.PageSize, ct);
            },
            cancellationToken:  cancellationToken
            );
        
        return invoices.Count == 0 
            ? throw new KeyNotFoundException("No invoices found") 
            : invoices.ToResponses();
    }
}