using Application.Features.CreateInvoice.Dtos.requests;
using Application.Features.CreateInvoice.Dtos.responses;

namespace Application.Features.CreateInvoice.Services;

public interface IInvoiceService
{
    public Task<InvoiceResponseDto> CreateAsync(InvoiceRequest invoiceRequest, CancellationToken cancellationToken);
}