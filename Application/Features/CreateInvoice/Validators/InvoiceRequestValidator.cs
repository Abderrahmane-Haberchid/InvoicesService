using Application.Features.CreateInvoice.Dtos.requests;
using FluentValidation;
using InvoicesService.Features.CreateInvoice.Validators;

namespace Application.Features.CreateInvoice.Validators;

public class InvoiceRequestValidator : AbstractValidator<InvoiceRequest>{
    public InvoiceRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("CustomerId must be greater than zero and not empty");
        
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("CompanyId must not be empty");

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage("Currency should not be empty");
        
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Items must not be empty");
        
        RuleForEach(x => x.Items)
            .SetValidator(new InvoiceItemRequestValidator());
        
        
        
    }
}