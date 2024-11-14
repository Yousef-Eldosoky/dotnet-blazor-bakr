using System.ComponentModel.DataAnnotations;

namespace Bakr.Shared.Dtos;

public abstract record CreateGenreDto
(
    [Required][StringLength(50)] string Name
);
