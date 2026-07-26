using InvoicesService.Features.CreateInvoice.Dtos;
using InvoicesService.Features.Dtos;

namespace InvoicesService.Features.CreateInvoice.Services;

public interface IInvoiceService
{
    public Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto invoiceDto, CancellationToken cancellationToken);
}