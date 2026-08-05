using Application.Features.CreateInvoice.Dtos.responses;

namespace Application.Features.GetInvoices;

public interface IGetInvoicesService
{
    Task<List<InvoiceResponseDto>> GetInvoicesByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    
    Task<InvoiceResponseDto>  GetInvoiceByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken);
    
    Task<List<InvoiceResponseDto>> GetAllInvoicesAsync(int take, int skip, CancellationToken cancellationToken);
}