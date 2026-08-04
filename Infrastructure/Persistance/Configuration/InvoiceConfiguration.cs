using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration;

public class InvoiceConfiguration :  IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.Property(i => i.Id)
            .ValueGeneratedNever();
        
        builder.HasKey(i => i.Id);
        
        builder.HasMany(i => i.Items)
            .WithOne(i => i.Invoice)
            .HasForeignKey(i => i.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.CompanyId);
    }
}