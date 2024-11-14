namespace Bakr.Shared.Dtos;

public record UpdateInvoiceDto
(
    List<int>? ProductsId,
    List<int>? ProductQuantity,
    decimal DiscountInPrice
);
