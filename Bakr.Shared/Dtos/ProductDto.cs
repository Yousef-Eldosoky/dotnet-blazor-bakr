namespace Bakr.Shared.Dtos;

public record ProductDto
(
    int Id,
    string Name,
    string Description,
    int GenreId,
    string? Picture,
    decimal Price,
    int Quantity
);