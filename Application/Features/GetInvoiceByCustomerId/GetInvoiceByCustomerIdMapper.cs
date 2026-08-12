using Application.Features.GetInvoiceByCustomerId;
using Domain.Models;

namespace Application.Features.GetInvoiceByCutomerId;

public static class GetInvoiceByCustomerIdMapper
{
    public static List<GetInvoiceByCustomerIdResponse> ToResponses(this List<Invoice> invoices)
    {
        return invoices.Select(i => new GetInvoiceByCustomerIdResponse(
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