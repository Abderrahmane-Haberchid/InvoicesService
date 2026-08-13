
using Application.Abstractions;
using Application.Common;
using Domain.Respository;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = FluentValidation.ValidationException;

namespace Application.Features.GetInvoices;

public class GetInvoicesHandler(
    IInvoiceRepository invoiceRepository,
    ICacheService cacheService,
    IValidator<GetInvoicesQuery> getInvoicesQueryValidator,
    ILogger<GetInvoicesHandler> logger) : IRequestHandler<GetInvoicesQuery, PagedList<GetInvoicesResponse>>
{

    public async Task<PagedList<GetInvoicesResponse>> Handle(
        GetInvoicesQuery request, 
        CancellationToken cancellationToken)
    {
        var result = await getInvoicesQueryValidator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var invoices = await cacheService.GetOrCreateAsync(
            $"cached-invoices-{request?.Page}-{request?.PageSize}",
            async ct =>
            {
                logger.LogInformation("Hitting database, no data available in cache!");
                var entities =  await invoiceRepository
                    .GetAllInvoicesAsync(request?.Page, request?.PageSize, ct);

                return entities.ToResponses();
            },
            cancellationToken:  cancellationToken
            );
        
        
        return invoices.Count == 0 
            ? throw new KeyNotFoundException("No invoices found") 
            : invoices.ToPagedList(request!.Page, request.PageSize);
    }
}