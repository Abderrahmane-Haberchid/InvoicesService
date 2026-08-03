using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InvoicesService.Enums;
using InvoicesService.Models;

namespace InvoicesService.Domain.Models;


public class Invoice
{
    [Key]
    [Column("id")]
    public Guid Id { get; init; }
    
    [Required]
    [Column("companyId")]
    public Guid CompanyId { get; init; }

    public List<InvoiceItem> Items { get; init; } = [];
    
    [Required]
    [Column("customerId")]
    public int CustomerId { get; init; }
    
    [Required]
    [Column("currency")]
    public CurrencyType Currency { get; init; }
    
    [Column("total")]
    public decimal Total { get; init; }
    
    [Column("createdAt")]
    public DateTime CreatedAt { get; init; }
    
    [Column("status")]
    public InvoiceStatus Status { get; init; }

}