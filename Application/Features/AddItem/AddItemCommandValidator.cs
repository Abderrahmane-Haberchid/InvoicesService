using FluentValidation;

namespace Application.Features.AddItem;

public class AddItemCommandValidator : AbstractValidator<AddItemCommand>
{
    public AddItemCommandValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty();
        
        RuleFor(x => x.Quantity)
            .GreaterThan(0);
        
        RuleFor(x => x.UnitPrice)
            .GreaterThan(0);
        
        RuleFor(x => x.ProductId)
            .GreaterThan(0);
    }
}