using FluentValidation;

namespace Application.Features.GetInvoiceByCustomerId;

public class GetInvoiceByCustomerIdQueryValidator : AbstractValidator<GetInvoiceByCustomerIdQuery>
{
    public GetInvoiceByCustomerIdQueryValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().GreaterThan(0);
    }
}