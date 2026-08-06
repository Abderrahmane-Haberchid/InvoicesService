
using Domain.Models;

namespace Application.Features.CreateInvoice;

public static class Mapper
{
    public static Response ToResponse(this Invoice invoice)
    {
        return new Response(
            invoice.Id,
            invoice.Status,
            invoice.Total,
            invoice.Currency,
            invoice.CreatedAt,
            invoice.GetItems().Select(item => new ItemResponse(item.ProductId, item.Quantity, item.UnitPrice)).ToList()
            );
    }
}