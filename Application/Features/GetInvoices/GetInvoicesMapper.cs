using Domain.Models;

namespace Application.Features.GetInvoices;

public static class GetInvoicesMapper
{
    public static List<GetInvoicesResponse> ToResponses(this List<Invoice> invoices)
    {
        return invoices.Select(i => new GetInvoicesResponse(
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