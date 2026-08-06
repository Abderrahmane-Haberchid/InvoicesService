using Domain.Models;

namespace Application.Features.GetInvoices;

public static class Mapper
{
    public static List<Response> ToResponses(this List<Invoice> invoices)
    {
        return invoices.Select(i => new Response(
                i.Id,
                i.Status,
                i.Total,
                i.Currency,
                i.CreatedAt,
                i.GetItems()
                    .Select(ii => new ItemResponse(ii.ProductId, ii.Quantity, ii.UnitPrice))
                    .ToList()))
            .ToList();
    }
}