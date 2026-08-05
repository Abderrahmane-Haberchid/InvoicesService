using Domain.Models;

namespace Domain.Respository;

public interface IInvoiceRepository
{
    public Task<Invoice> CreateInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken);

    public Task<List<Invoice>> GetInvoiceByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    
    public Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken);
    
    public Task<List<Invoice>> GetAllInvoicesAsync(int take, int skip, CancellationToken cancellationToken);
    
    public Task SaveChangeAsync(CancellationToken cancellationToken);
}