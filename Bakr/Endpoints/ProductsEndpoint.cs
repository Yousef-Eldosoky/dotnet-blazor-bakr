using Bakr.Data;
using Bakr.Shared.Dtos;
using Bakr.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Bakr.Mapping;

namespace Bakr.Endpoints;

public static class ProductsEndpoint
{
    const string getProductEndpointName = "GetProduct";
    // public static readonly List<ProductDto> productDtos = [
    //     new(1, "Joo", "ddd", "", "ddf.jpg", 19.99M, 3),
    //     new(2, "Yousef", "fff", "", "zdxfv.jpg", 25.49M, 6),
    // ];
    public static RouteGroupBuilder MapProductsEndpoint(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/products").WithParameterValidation().RequireAuthorization();

        group.MapGet("/", async (BakrDbContext dbContext) =>
        {
            // await Task.Delay(3000);
            return Results.Ok(await dbContext.Products.Include(product => product.Genre).AsNoTracking().ToListAsync());
        });

        group.MapGet("/{id}", async (int id, BakrDbContext dbContext) =>
        {
            Product? product = await dbContext.Products.FindAsync(id);
            if (product is null) return Results.NotFound();
            if (product.GenreId != null) product.Genre = await dbContext.Genres.FindAsync(product.GenreId);
            return Results.Ok(product.ToDto());
        }).WithName(getProductEndpointName);

        group.MapPost("/", async (CreateProductDto newProduct, BakrDbContext dbContext) =>
        {
            Product product = newProduct.ToEntity(null);
            if (product.GenreId != null) product.Genre = await dbContext.Genres.FindAsync(newProduct.GenreId);
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();
            return Results.CreatedAtRoute(getProductEndpointName, new { id = product.Id }, product.ToDto());
        }).RequireAuthorization("AdminPolicy");

        group.MapPut("/{id}", async (int id, CreateProductDto newProduct, BakrDbContext dbContext) =>
        {
            Product? product = await dbContext.Products.FindAsync(id);
            if (product is null) return Results.NotFound();
            dbContext.Products.Entry(product).CurrentValues.SetValues(newProduct.ToEntity(id));
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization("AdminPolicy");

        group.MapDelete("/{id}", async (int id, BakrDbContext dbContext) =>
        {
            ProductInvoice? productInvoice = await dbContext.ProductInvoices.Where(p => p.ProductId == id).FirstOrDefaultAsync();
            if (productInvoice is not null) return Results.BadRequest("Item has been found in invoice.");
            await dbContext.Products.Where(product => product.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        }).RequireAuthorization("AdminPolicy");
        return group;
    }
}
