using Bakr.Data;
using Bakr.Shared.Dtos;
using Bakr.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Bakr.Mapping;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bakr.Endpoints;

public static class ProductsEndpoint
{
    // public static readonly List<ProductDto> productDtos = [
    //     new(1, "Joo", "ddd", "", "ddf.jpg", 19.99M, 3),
    //     new(2, "Yousef", "fff", "", "zdxfv.jpg", 25.49M, 6),
    // ];
    
    public static RouteGroupBuilder MapProductsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/products").WithParameterValidation().RequireAuthorization();

        group.MapGet("/", GetProductsAsync);

        group.MapGet("/{id:int}", GetProductAsync).WithName(nameof(GetProductAsync));

        group.MapPost("/", PostProductAsync).RequireAuthorization("AdminPolicy");

        group.MapPut("/{id:int}", PutProductAsync).RequireAuthorization("AdminPolicy");

        group.MapDelete("/{id:int}", DeleteProductAsync).RequireAuthorization("AdminPolicy");
        
        return group;
    }
    
    public static async Task<Ok<List<Product>>> GetProductsAsync(ApplicationDbContext dbContext)
    {
        // await Task.Delay(3000);
        return TypedResults.Ok(await dbContext.Products.Include(product => product.Genre).AsNoTracking().ToListAsync());
    }

    public static async Task<Results<Ok<ProductDto>, NotFound>> GetProductAsync(int id, ApplicationDbContext dbContext)
    {
        Product? product = await dbContext.Products.FindAsync(id);
        if (product is null) return TypedResults.NotFound();
        if (product.GenreId != null) product.Genre = await dbContext.Genres.FindAsync(product.GenreId);
        return TypedResults.Ok(product.ToDto());
    }
    
    private static async Task<CreatedAtRoute> PostProductAsync(CreateProductDto newProduct, ApplicationDbContext dbContext)
    {
        Product product = newProduct.ToEntity(null);
        if (product.GenreId != null) product.Genre = await dbContext.Genres.FindAsync(newProduct.GenreId);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return TypedResults.CreatedAtRoute(nameof(GetProductAsync), new {id = product.Id});
    }
    
    private static async Task<Results<NotFound, NoContent>> PutProductAsync(int id, CreateProductDto newProduct, ApplicationDbContext dbContext, IHostEnvironment env)
    {
        Product? product = await dbContext.Products.FindAsync(id);
        if (product is null) return TypedResults.NotFound();
        if(!product.Picture.IsNullOrEmpty() && product.Picture != newProduct.Picture)
            File.Delete(Path.Combine(env.ContentRootPath, "Assets", "Products", "unsafe_uploads", product.Picture!));
        dbContext.Products.Entry(product).CurrentValues.SetValues(newProduct.ToEntity(id));
        await dbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }
    
    private static async Task<Results<BadRequest<string>, NoContent>> DeleteProductAsync(int id, ApplicationDbContext dbContext, IHostEnvironment env)
    {
        ProductInvoice? productInvoice = await dbContext.ProductInvoices.Where(p => p.ProductId == id).FirstOrDefaultAsync();
        if (productInvoice is not null) return TypedResults.BadRequest("Item has been found in invoice.");
        Product? product = await dbContext.Products.FindAsync(id);
        if (product is not null)
        {
            await dbContext.Products.Where(p => p.Id == id).ExecuteDeleteAsync();
            if (!product.Picture.IsNullOrEmpty())
                File.Delete(Path.Combine(env.ContentRootPath, "Assets", "Products", "unsafe_uploads", product.Picture!));
        }
        return TypedResults.NoContent();
    }
}
