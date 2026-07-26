using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InvoicesService.Enums;

namespace InvoicesService.Models;

[Table("Invoices")]
public class Invoices
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("customerId")]
    public Guid CustomerId { get; set; }
    
    [Required]
    [Column("currency")]
    public CurrencyType Currency { get; set; }
    
    public InvoiceItems InvoiceItems { get; set; }
    
    [Column("total")]
    public decimal Total { get; set; }
    
    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [Column("status")]
    public InvoiceStatus Status { get; set; }

}