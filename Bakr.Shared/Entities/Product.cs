using Microsoft.EntityFrameworkCore;

namespace Bakr.Shared.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public int? GenreId { get; set; }

    public Genre? Genre { get; set; }

    public string Description { get; set; } = "";

    public string? Picture { get; set; }

    [Precision(18, 2)]
    public required decimal Price { get; set; }

    public int Quantity { get; set; }

}
