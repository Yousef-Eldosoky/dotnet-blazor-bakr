using System.ComponentModel.DataAnnotations;

namespace Bakr.Shared.Dtos;

public record class CreateProductDto
(
    [Required][StringLength(50)] string Name,
    string? Description,
    int? GenreId,
    string? Picture,
    [Range(1, 1000000)] decimal Price,
    [Range(0, 100000)]int Quantity
);
