using Application.Features.CreateInvoice.Dtos.requests;
using InvoicesService.Features.CreateInvoice.Dtos;
using InvoicesService.Features.CreateInvoice.Dtos.responses;

namespace InvoicesService.Features.CreateInvoice.Services;

public interface IInvoiceService
{
    public Task<InvoiceResponseDto> CreateAsync(InvoiceRequest invoiceRequest, CancellationToken cancellationToken);
}