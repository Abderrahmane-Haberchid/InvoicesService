using Domain.Models;

namespace Domain.Respository;

public interface IInvoiceRepository
{
    public Task<Invoice> CreateInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken);
    
    public Task SaveChangeAsync(CancellationToken cancellationToken);
}