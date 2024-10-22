using System.ComponentModel.DataAnnotations;

namespace Bakr.Dtos;

public record class CreateGenreDto
(
    [Required][StringLength(50)] string Name
);
