using Application.Exceptions;
using Domain.Enums;
using Domain.Models;
using Domain.Respository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistance;

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

    public Task<List<Invoice>> GetInvoiceByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
    {
        return dbContext.Invoices
            .AsNoTracking()
            .Include(i => i.Items)
            .Where(i => i.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        return await dbContext.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == invoiceId,  cancellationToken);
    }

    public async Task<Invoice?> GetInvoiceByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await dbContext.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.CompanyId == companyId, cancellationToken);
    }

    private IQueryable<Invoice> GetInvoices()
    {
        return dbContext.Invoices
            .AsNoTracking()
            .Include(i => i.Items)
            .AsQueryable();
    }
    public Task<List<Invoice>> GetAllInvoicesAsync(int? page, int? pageSize, CancellationToken cancellationToken)
    {
        
        if (!page.HasValue || !pageSize.HasValue)
        {
            return GetInvoices().ToListAsync(cancellationToken);
        }
        
        return  GetInvoices()
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync(cancellationToken);
        
    }

    public async Task SaveChangeAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}