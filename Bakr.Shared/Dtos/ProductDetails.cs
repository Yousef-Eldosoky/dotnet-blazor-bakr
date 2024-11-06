using System.ComponentModel.DataAnnotations;

namespace Bakr.Shared.Dtos;

public class ProductDetails
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public required string Name { get; set; }

    public string? Picture { get; set; }

    public string Description { get; set; } = "";

    [Required]
    public int? GenreId { get; set; }

    [Range(1, 1000000)]
    public decimal Price { get; set; }

    [Range(1, 100000)]
    public int Quantity { get; set; }
}
