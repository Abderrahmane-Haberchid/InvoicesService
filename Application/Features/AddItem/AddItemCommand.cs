using MediatR;

namespace Application.Features.AddItem;

public record AddItemCommand(
    Guid InvoiceId,
    int ProductId, 
    int Quantity, 
    decimal UnitPrice) : IRequest<AddItemResponse>;