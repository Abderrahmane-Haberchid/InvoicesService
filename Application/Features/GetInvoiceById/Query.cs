using MediatR;

namespace Application.Features.GetInvoiceById;

public record Query(Guid invoiceId) : IRequest<Response>;