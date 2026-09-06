using Domain.Respository;
using FluentValidation;
using MediatR;

namespace Application.Features.GetInvoiceByCustomerId;

public class GetInvoiceByCustomerIdHandler(
    IInvoiceRepository invoiceRepository,
    IValidator<GetInvoiceByCustomerIdQuery> validator) 
    : IRequestHandler<GetInvoiceByCustomerIdQuery, List<GetInvoiceByCustomerIdResponse>>
{
    public async Task<List<GetInvoiceByCustomerIdResponse>> Handle(GetInvoiceByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var result = await validator.ValidateAsync(request, cancellationToken);
        
        if(!result.IsValid)
            throw new  ValidationException(result.Errors);

        var invoices = await invoiceRepository.GetInvoiceByCustomerIdAsync(request.CustomerId, cancellationToken);

        return invoices.Count == 0 
            ? throw new KeyNotFoundException("Invoices Not Found") 
            : invoices.ToResponses();
    }
}