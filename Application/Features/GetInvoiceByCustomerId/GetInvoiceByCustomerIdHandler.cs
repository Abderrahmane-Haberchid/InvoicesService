
using Domain.Respository;
using MediatR;

namespace Application.Features.GetInvoiceByCutomerId;

public class GetInvoiceByCustomerIdHandler(IInvoiceRepository invoiceRepository) : IRequestHandler<GetInvoiceByCustomerIdQuery, List<GetInvoiceByCustomerIdResponse>>
{
    public async Task<List<GetInvoiceByCustomerIdResponse>> Handle(GetInvoiceByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        if (request.CustomerId <= 0)
        {
            throw new ArgumentNullException(nameof(request.CustomerId), "CustomerId Should not be Empty");
        }

        var invoices = await invoiceRepository.GetInvoiceByCustomerIdAsync(request.CustomerId, cancellationToken);

        if (invoices.Count == 0)
        {
            throw new KeyNotFoundException("Invoices Not Found");
        }

        return invoices.ToResponses();
    }
}