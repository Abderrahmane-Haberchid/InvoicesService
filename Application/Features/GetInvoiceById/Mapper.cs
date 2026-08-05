using Domain.Models;

namespace Application.Features.GetInvoiceById;

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
            invoice.GetItems().Select(i => new InvoiceItemQuery(i.ProductId, i.Quantity, i.UnitPrice)).ToList());
    }
}