using Domain.Respository;
using FluentValidation;
using MediatR;

namespace Application.Features.GetInvoiceById;

public class GetInvoiceByIdHandler(
    IInvoiceRepository invoiceRepository,
    IValidator<GetInvoiceByIdQuery> validator) 
    : IRequestHandler<GetInvoiceByIdQuery, GetInvoiceByIdResponse>
{
    public async Task<GetInvoiceByIdResponse> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var validate = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validate.IsValid)
            throw new ValidationException("Invalid invoice id");

        var invoice = await invoiceRepository.GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);
        
        return invoice == null 
            ? throw new KeyNotFoundException($"No invoice found with Id :  '{request.InvoiceId}'")
            : invoice.ToResponse();
    }
}