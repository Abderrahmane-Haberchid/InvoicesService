using Application.Exceptions;
using Domain.Models.Invoice;

namespace Application.Features.AddItem;

public static class AddItemMapper
{
    public static AddItemResponse ToAddItemResponse(this InvoiceItem item)
    {
        if (item.Invoice is null)
        {
            throw new NullItemFromInvoiceException("Cant resolve invoice from item in Mapper class.");
        }
        return new AddItemResponse(
            item.InvoiceId,
            item.ProductId,
            item.Quantity,
            item.Invoice.Total,
            item.Invoice.UpdatedAt);
    }
}