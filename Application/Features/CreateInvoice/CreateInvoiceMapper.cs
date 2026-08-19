
using Domain.Models;

namespace Application.Features.CreateInvoice;

public static class CreateInvoiceMapper
{
    public static CreateInvoiceResponse ToResponse(this Invoice invoice)
    {
        return new CreateInvoiceResponse(
            invoice.Id,
            invoice.CompanyId,
            invoice.Status,
            invoice.Total,
            invoice.Currency,
            invoice.CreatedAt,
            invoice.GetItems().Select(item => new ItemResponse(item.ProductId, item.Quantity, item.UnitPrice)).ToList()
            );
    }
}