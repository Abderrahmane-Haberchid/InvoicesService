using Application.Features.GetInvoiceByCutomerId;
using MediatR;

namespace Application.Features.GetInvoiceByCustomerId;

public record GetInvoiceByCustomerIdQuery(int CustomerId) : IRequest<List<GetInvoiceByCustomerIdResponse>>;