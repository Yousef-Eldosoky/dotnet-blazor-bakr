using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bakr.Shared.Entities;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    public DateTime DateTime { get; set; } = DateTime.Now;

    public required string UserId { get; set; }

    [ForeignKey("UserId")]
    public required string Name { get; set; }

    [Precision(18, 2)]
    public decimal DiscountInPrice { get; set; }

    [Precision(18, 2)]
    public List<ProductInvoice> ProductInvoices { get; set; } = [];

}
