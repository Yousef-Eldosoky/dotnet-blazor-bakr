namespace Bakr.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public int? GenreId { get; set; }

    public Genre? Genre { get; set; }

    public string Description { get; set; } = "";

    public string? Picture { get; set; }

    public required decimal Price { get; set; }

    public int Quantity { get; set; } = 0;

}
