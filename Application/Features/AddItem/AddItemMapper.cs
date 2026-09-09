using Domain.Models.Invoice;

namespace Application.Features.AddItem;

public static class AddItemMapper
{
    public static AddItemResponse ToAddItemResponse(this InvoiceItem item)
    {
        return new AddItemResponse(
            item.InvoiceId,
            item.ProductId,
            item.Quantity,
            item.Invoice.Total,
            item.Invoice.UpdatedAt);
    }
}