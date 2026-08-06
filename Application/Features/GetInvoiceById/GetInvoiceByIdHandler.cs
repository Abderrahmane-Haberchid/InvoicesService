using Domain.Respository;
using MediatR;

namespace Application.Features.GetInvoiceById;

public class GetInvoiceByIdHandler(IInvoiceRepository invoiceRepository) : IRequestHandler<GetInvoiceByIdQuery, GetInvoiceByIdResponse>
{
    public async Task<GetInvoiceByIdResponse> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.InvoiceId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(request.InvoiceId), "Invoice Id should not be Empty");
        }

        var invoice = await invoiceRepository.GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);

        if (invoice is null)
        {
            throw new KeyNotFoundException($"No invoice found with Id :  '{request.InvoiceId}'");
        }

        return invoice.ToResponse();
    }
}