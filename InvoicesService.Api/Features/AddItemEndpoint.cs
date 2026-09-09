using Application.Features.AddItem;
using MediatR;

namespace InvoicesService.Features;

public static class AddItemEndpoint
{
    public static void MapAddItemEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1/invoices/add-item", async (
            ISender sender,
            AddItemCommand command,
        CancellationToken ct
        ) =>
        {
            
            var result = await sender.Send(command, ct);
            
            return TypedResults.Ok(result);
        });
    }
}