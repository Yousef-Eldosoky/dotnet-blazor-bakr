using Bakr.Entities;

namespace Bakr.Dtos;

public record class InvoiceDto
(
    int Id,
    string UserName,
    List<ProductInvoice> ProductsInvoice,
    DateTime DateTime,
    decimal DiscountInPrice
);
