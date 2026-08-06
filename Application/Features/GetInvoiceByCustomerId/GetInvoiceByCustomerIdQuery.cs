using MediatR;

namespace Application.Features.GetInvoiceByCutomerId;

public record GetInvoiceByCustomerIdQuery(int CustomerId) : IRequest<List<GetInvoiceByCustomerIdResponse>>;