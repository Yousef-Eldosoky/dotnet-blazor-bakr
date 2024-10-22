using System.ComponentModel.DataAnnotations;

namespace Bakr.Entities;

public class Genre
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
}
