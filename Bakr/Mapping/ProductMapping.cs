using Bakr.Shared.Dtos;
using Bakr.Shared.Entities;

namespace Bakr.Mapping;

public static class ProductMapping
{
    public static Product ToEntity(this CreateProductDto product, int? id)
    {
        if (id is null)
            return new Product
            {
                Name = product.Name,
                GenreId = product.GenreId,
                Description = product.Description ?? "",
                Picture = product.Picture,
                Price = product.Price,
                Quantity = product.Quantity,
            };
        return new Product
        {
            Id = (int)id,
            Name = product.Name,
            GenreId = product.GenreId,
            Description = product.Description ?? "",
            Picture = product.Picture,
            Price = product.Price,
            Quantity = product.Quantity,
        };
    }

    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Genre?.Id ?? 0,
            product.Picture,
            product.Price,
            product.Quantity
        );
    }

    public static ProductDetails ToDetails(this ProductDto product) {
        return new() {
            Description = product.Description,
            GenreId = product.GenreId,
            Name = product.Name,
            Id = product.Id,
            Picture = product.Picture,
            Price = product.Price,
            Quantity = product.Quantity
        };
    }
}
