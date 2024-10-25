using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bakr.Shared.Entities;

[PrimaryKey(nameof(InvoiceId), nameof(ProductId))]
public class ProductInvoice
{
    [ForeignKey("Invoice")]
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
