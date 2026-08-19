using FluentValidation;

namespace Application.Features.CreateInvoice;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("CustomerId must be greater than zero and not empty");

        RuleFor(x => x.Currency)
            .IsInEnum()
            .WithMessage("Currency should not be empty");
        
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Items must not be empty");
            
        RuleForEach(x => x.Items)
            .SetValidator(new CreateInvoiceItemValidator());
    }
}

public class CreateInvoiceItemValidator : AbstractValidator<InvoiceItemCommand>
{
    public CreateInvoiceItemValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("UnitPrice must be greater than 0");

        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId must be valid");
    }
}
