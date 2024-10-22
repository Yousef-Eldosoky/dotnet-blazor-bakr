using System.ComponentModel.DataAnnotations;

namespace Bakr.Dtos;

public record class CreateInvoiceDto(
    [Required] List<int> ProductsId,
    [Required] List<int> ProductQuantity,
    decimal? DiscountInPrice
);