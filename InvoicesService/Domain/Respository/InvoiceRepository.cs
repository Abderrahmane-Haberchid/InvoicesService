using InvoicesService.DbContext;
using InvoicesService.Domain.Models;
using InvoicesService.Models;

namespace InvoicesService.Respository;

public class InvoiceRepository(
    AppDbContext dbContext,
    ILogger<InvoiceRepository> logger)
    : IInvoiceRepository
{
    public async Task<Invoice> CreateInvoiceAsync(
        Invoice invoice,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("1. Processing saving {invoiceId} to database", invoice.Id);

        ArgumentNullException.ThrowIfNull(invoice);
        
        var saved = await dbContext.Invoices.AddAsync(invoice, cancellationToken);
        
        logger.LogInformation("2. invoice saved {invoice} to database", invoice.Id);
        return saved.Entity;
    }

    public async Task SaveChangeAsync(CancellationToken cancellationToken)
    {
        await  dbContext.SaveChangesAsync(cancellationToken);
    }
}