using System.ComponentModel.DataAnnotations;

namespace Bakr.Shared.Dtos;

public record class CreateGenreDto
(
    [Required][StringLength(50)] string Name
);
