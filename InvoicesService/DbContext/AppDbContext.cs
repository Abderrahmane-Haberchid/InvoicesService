using InvoicesService.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoicesService.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Invoices> Invoices { get; set; }
    public DbSet<InvoiceItems> InvoiceItems { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
}