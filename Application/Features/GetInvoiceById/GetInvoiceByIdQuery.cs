using MediatR;

namespace Application.Features.GetInvoiceById;

public record GetInvoiceByIdQuery(Guid InvoiceId) : IRequest<GetInvoiceByIdResponse>;