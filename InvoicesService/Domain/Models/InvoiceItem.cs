using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InvoicesService.Domain.Models;

namespace InvoicesService.Models;

public class InvoiceItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Column("invoiceId")]
    public Guid InvoiceId { get; set; }

    public Invoice Invoice { get; set; } = null;
    
    [Column("productId")]
    public int ProductId { get; set; }
    
    [Column("quantity")]
    [Required]
    public int Quantity { get; set; }
    
    [Column("unitPrice")]
    [Required]
    public decimal UnitPrice { get; set; }
}