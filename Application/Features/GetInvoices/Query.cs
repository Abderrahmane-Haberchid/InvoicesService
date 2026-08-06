using MediatR;

namespace Application.Features.GetInvoices;

public record Query(int Page, int PageSize) : IRequest<List<Response>>;