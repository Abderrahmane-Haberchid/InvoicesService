using Application.Common;
using MediatR;

namespace Application.Features.GetInvoices;

public record GetInvoicesQuery(int Page , int PageSize) : IRequest<PagedList<GetInvoicesResponse>>;