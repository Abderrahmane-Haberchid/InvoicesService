using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InvoicesService.Enums;

namespace InvoicesService.Models;

[Table("Invoices")]
public class Invoice
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    public List<InvoiceItems> Items { get; set; }
    
    [Required]
    [Column("customerId")]
    public int CustomerId { get; set; }
    
    [Required]
    [Column("currency")]
    public CurrencyType Currency { get; set; }
    
    [Column("total")]
    public decimal Total { get; set; }
    
    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [Column("status")]
    public InvoiceStatus Status { get; set; }

}