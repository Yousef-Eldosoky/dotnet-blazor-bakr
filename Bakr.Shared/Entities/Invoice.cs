using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bakr.Shared.Entities;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    public DateTime DateTime { get; set; } = DateTime.Now;

    public required string UserId { get; set; }

    [ForeignKey("UserId")]
    public required string Name { get; set; }

    public decimal DiscountInPrice { get; set; } = 0;

    public List<ProductInvoice> ProductInvoices { get; set; } = [];

}
