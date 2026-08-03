using InvoicesService.Domain.Models;
using InvoicesService.Models;

namespace InvoicesService.Respository;

public interface IInvoiceRepository
{
    public Task<Invoice> CreateInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken);
    
    public Task SaveChangeAsync(CancellationToken cancellationToken);
}