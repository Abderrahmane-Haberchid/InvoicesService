using Application.Common;
using Domain.Models.Invoice;

namespace Application.Features.GetInvoices;

public static class GetInvoicesMapper
{
    public static List<GetInvoicesResponse> ToResponses(this List<Invoice> invoices)
    {
        return invoices.Select(i => new GetInvoicesResponse(
                i.Id,
                i.CompanyId,
                i.Status,
                i.Total,
                i.Currency,
                i.CreatedAt,
                i.GetItems()
                    .Select(ii => new ItemResponse(ii.ProductId, ii.Quantity, ii.UnitPrice))
                    .ToList()))
            .ToList();
    }

    public static PagedList<GetInvoicesResponse> ToPagedList(this List<GetInvoicesResponse> invoices, int page, int pageSize)
    {
        return new PagedList<GetInvoicesResponse>
        {
            Items = invoices,
            TotalCount = invoices.Count,
            PageSize = pageSize,
            PageNumber = page
        };
    }
}