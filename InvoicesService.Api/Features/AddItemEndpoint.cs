using Application.Features.AddItem;
using MediatR;

namespace InvoicesService.Features;

public static class AddItemEndpoint
{
    public static void MapAddItemEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/invoices/{invoiceId}/items", async (
            ISender sender,
            AddItemCommand command,
            Guid invoiceId,
        CancellationToken ct
        ) =>
        {
            
            var result = await sender.Send(command, ct);
            
            return TypedResults.Ok(result);
        });
    }
}