using Domain.Models.Invoice;
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

        var invoice = await invoiceRepository
            .GetInvoiceByIdAsync(request.InvoiceId, cancellationToken);

        if (invoice == null)
        {
            logger.LogWarning("Invoice not found {InvoiceId} at: {dateTime}", request.InvoiceId,  DateTime.UtcNow);
            throw new KeyNotFoundException("Invoice not found");
        }

        var invoiceItem = InvoiceItem.Create(
            request.InvoiceId,
            invoice,
            request.ProductId,
            request.Quantity,
            request.UnitPrice);
        
        invoice.AddInvoiceItem(invoiceItem);

        await invoiceRepository.SaveChangeAsync(cancellationToken);

        var item = invoice.GetItem(request.ProductId)!;
        return item.ToAddItemResponse();
    }
}