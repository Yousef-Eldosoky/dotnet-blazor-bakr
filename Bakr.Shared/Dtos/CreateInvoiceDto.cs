using System.ComponentModel.DataAnnotations;

namespace Bakr.Shared.Dtos;

public record CreateInvoiceDto(
    [Required] List<int> ProductsId,
    [Required] List<int> ProductQuantity,
    decimal? DiscountInPrice
);