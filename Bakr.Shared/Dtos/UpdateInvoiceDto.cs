namespace Bakr.Shared.Dtos;

public record class UpdateInvoiceDto
(
    List<int>? ProductsId,
    List<int>? ProductQuantity,
    decimal DiscountInPrice
);
