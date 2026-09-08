using Domain.Respository;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.AddItem;

public class AddItemHandler(
    IValidator<AddItemCommand> validator,
    ILogger<AddItemHandler> logger,
    IInvoiceRepository invoiceRepository) : IRequestHandler<AddItemCommand, AddItemResponse>
{
    public async Task<AddItemResponse> Handle(
        AddItemCommand request, 
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            logger.LogError("Validation Failed {AddItemCommand}", result.Errors);
            throw new ValidationException("Unable to validate AddItemCommand");
        }
    }
}