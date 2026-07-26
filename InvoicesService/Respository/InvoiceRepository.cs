using InvoicesService.DbContext;
using InvoicesService.Models;

namespace InvoicesService.Respository;

public class InvoiceRepository(
    AppDbContext dbContext,
    ILogger<InvoiceRepository> logger) 
    : IInvoiceRepository
{
    public async Task<Invoice> CreateInvoiceAsync(
        Invoice invoice,
        List<InvoiceItems> invoiceItems,
        CancellationToken cancellationToken)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            logger.LogInformation("1. Processing saving {invoice} to database", invoice);
            ArgumentNullException.ThrowIfNull(invoice);
            ArgumentNullException.ThrowIfNull(invoiceItems);

            var savedInvoice = await dbContext.Invoices
                .FindAsync(invoice.Id, cancellationToken);
        
            if(savedInvoice is not null)
                throw new ArgumentException("Invoice already exists", nameof(invoice));
        
            await dbContext.Invoices.AddAsync(invoice, cancellationToken);
            await dbContext.AddAsync(invoiceItems, cancellationToken);
            
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation("2. invoice saved {invoice} to database", invoice);
            return invoice;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError("Unable to save invoice to database {error}", e.Message);
            throw;
        }
    }
}