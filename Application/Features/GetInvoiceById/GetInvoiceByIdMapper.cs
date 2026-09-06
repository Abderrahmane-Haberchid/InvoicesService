
using Domain.Models.Invoice;

namespace Application.Features.GetInvoiceById;

public static class GetInvoiceByIdMapper
{
    public static GetInvoiceByIdResponse ToResponse(this Invoice invoice)
    {
        return new GetInvoiceByIdResponse(
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