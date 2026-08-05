using MediatR;

namespace Application.Features.GetInvoiceByCutomerId;

public record Query(int CustomerId) : IRequest<List<Response>>;