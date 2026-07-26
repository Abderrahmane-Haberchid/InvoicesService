using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicesService.Models;

[Table("invoiceItems")]
public class InvoiceItems
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InvoiceItemId { get; set; }
    
    [Column("invoiceId")]
    public Guid InvoiceId { get; set; }
    
    [Column("customerId")]
    public Guid CustomerId { get; set; }
    
    [Column("quantity")]
    [Required]
    public int Quantity { get; set; }
    
    [Column("unitPrice")]
    [Required]
    public decimal UnitPrice { get; set; }
}