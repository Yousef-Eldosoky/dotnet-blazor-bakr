using Bakr.Shared.Entities;

namespace Bakr.Shared.Dtos;

public record InvoiceDto
(
    int Id,
    string UserName,
    List<ProductInvoice> ProductsInvoice,
    DateTime DateTime,
    decimal DiscountInPrice
);
