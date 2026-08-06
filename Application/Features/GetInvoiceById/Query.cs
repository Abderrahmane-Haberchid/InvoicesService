using MediatR;

namespace Application.Features.GetInvoiceById;

public record Query(Guid InvoiceId) : IRequest<Response>;