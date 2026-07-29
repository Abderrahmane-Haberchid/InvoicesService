using FluentValidation;
using InvoicesService.Enums;

namespace InvoicesService.Features.CreateInvoice.Dtos.requests;

public record CreateInvoiceDto(
    int CustomerId,
    CurrencyType Currency,
    List<CreateInvoiceItemDto> Items
);

public class Validator : AbstractValidator<CreateInvoiceDto>{
    public Validator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId Should not be empty")
            .GreaterThan(0).WithMessage("CustomerId must be greater than zero and not empty");

        RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency should not be empty");
        RuleFor(x => x.Items).NotNull().NotEmpty().WithMessage("Items can not be empty");
        
    }
}