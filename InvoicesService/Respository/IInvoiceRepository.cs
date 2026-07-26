using InvoicesService.Features.Dtos;
using InvoicesService.Models;

namespace InvoicesService.Respository;

public interface IInvoiceRepository
{
    public Task<Invoice> CreateInvoiceAsync(
        Invoice invoice,
        List<InvoiceItems> invoiceItems,
        CancellationToken cancellationToken);
}