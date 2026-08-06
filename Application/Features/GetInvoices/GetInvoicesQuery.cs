using MediatR;

namespace Application.Features.GetInvoices;

public record GetInvoicesQuery(int Page, int PageSize) : IRequest<List<GetInvoicesResponse>>;