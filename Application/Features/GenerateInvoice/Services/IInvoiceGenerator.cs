
namespace InvoicesService.Features.GenerateInvoice.Services;

public interface IInvoiceGenerator
{
    public Task InvoiceGeneratorAsync(Guid invoiceId, CancellationToken cancellationToken);
}