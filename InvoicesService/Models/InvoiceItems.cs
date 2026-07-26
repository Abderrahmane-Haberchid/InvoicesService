using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoicesService.Models;

[Table("invoiceItems")]
public class InvoiceItems
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Column("invoiceId")]
    public Guid InvoiceId { get; set; }
    
    [Column("productId")]
    public Guid ProductId { get; set; }
    
    [Column("quantity")]
    [Required]
    public int Quantity { get; set; }
    
    [Column("unitPrice")]
    [Required]
    public decimal UnitPrice { get; set; }
}