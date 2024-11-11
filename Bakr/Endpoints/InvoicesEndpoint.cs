using System.Security.Claims;
using Bakr.Data;
using Bakr.Shared.Dtos;
using Bakr.Shared.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Bakr.Endpoints;

public static class InvoicesEndpoint
{
    public static RouteGroupBuilder MapInvoiceEndpoint(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/invoices").WithParameterValidation().RequireAuthorization();

        group.MapGet("/", GetInvoicesAsync);

        group.MapPost("/", PostInvoiceAsync);

        group.MapGet("/{id:int}", GetInvoiceAsync).WithName(nameof(GetInvoiceAsync));

        // Unfinished function
        /**
        group.MapPut("/{id}", async (int id, UpdateInvoiceDto newInvoice, ClaimsPrincipal claims, BakrContext dbContext)=>{
            string userId = claims.Claims.First(c=> c.Type == ClaimTypes.NameIdentifier).Value;
            if (await dbContext.UserRoles.FindAsync(userId, 1) == null) return Results.Forbid();
            Invoice? invoice = await dbContext.Invoices.FindAsync(id);
            if (invoice is null) return Results.NotFound();
            invoice.DiscountInPrice = newInvoice.DiscountInPrice;
            
            List<ProductInvoice> productInvoices = await dbContext.ProductInvoices.Where(productInvoice => productInvoice.InvoiceId == id).AsNoTracking().ToListAsync();
            foreach (ProductInvoice productInvoice in productInvoices)
            {
                Product? product = await dbContext.Products.FindAsync(productInvoice.ProductId);
                product!.Quantity += productInvoice.Quantity;
                dbContext.Products.Entry(product).CurrentValues.SetValues(product);
            }
            return Results.NoContent();
        });
        */

        group.MapDelete("/{id:int}", DeleteInvoiceAsync).RequireAuthorization("AdminPolicy");
        
        return group;
    }
    
    private static async Task<Results<NotFound, Ok<InvoiceDto>>> GetInvoiceAsync(int id, ApplicationDbContext dbContext) 
    {
        Invoice? invoice = await dbContext.Invoices.Include(i => i.ProductInvoices).FirstOrDefaultAsync(i => i.Id == id);
        if (invoice is null) return TypedResults.NotFound();
        InvoiceDto invoiceDto = new(
            id,
            invoice.Name,
            invoice.ProductInvoices,
            invoice.DateTime,
            invoice.DiscountInPrice
        );
        return TypedResults.Ok(invoiceDto);
    }
    
    private static async Task<Ok<List<Invoice>>> GetInvoicesAsync(ApplicationDbContext dbContext)
    {
        return TypedResults.Ok(await dbContext.Invoices.Include(p => p.ProductInvoices).AsNoTracking().ToListAsync());
    }
    
    private static async Task<Results<BadRequest<string>, NotFound<string>, CreatedAtRoute>> PostInvoiceAsync(CreateInvoiceDto newInvoice, ApplicationDbContext dbContext, ClaimsPrincipal claims)
    {
        string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        if (newInvoice.ProductQuantity.Count != newInvoice.ProductsId.Count || newInvoice.ProductQuantity.Count == 0)
        {
            return TypedResults.BadRequest("The quantity array not equal to the product array or either of them is empty.");
        }
        string name = claims.Claims.First(c => c.Type == "FullName").Value;
        Invoice invoice = new()
        {
            UserId = userId,
            DiscountInPrice = newInvoice.DiscountInPrice ?? 0,
            Name = name,
            ProductInvoices = [],
        };
        for (int i = 0; i < newInvoice.ProductQuantity.Count; i++)
        {
            if (newInvoice.ProductQuantity[i] < 1) return TypedResults.BadRequest("The quantity of a product can not be less than 1.");
            Product? product = await dbContext.Products.FindAsync(newInvoice.ProductsId[i]);
            if (product is null) return TypedResults.NotFound("One or more Item is not Found in the database.");
            if (newInvoice.ProductQuantity[i] > product.Quantity) return TypedResults.BadRequest($"Quantity of {product.Name} is not enough.");
            invoice.ProductInvoices.Add(new ProductInvoice
            {
                ProductId = product.Id,
                InvoiceId = invoice.Id,
                Price = product.Price,
                Quantity = newInvoice.ProductQuantity[i],
            });
            product.Quantity -= newInvoice.ProductQuantity[i];
            dbContext.Products.Entry(product).CurrentValues.SetValues(product);
        }
        dbContext.ProductInvoices.AddRange(invoice.ProductInvoices);
        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync();
        return TypedResults.CreatedAtRoute(nameof(GetInvoiceAsync), new { id = invoice.Id });
    }
    
    private static async Task<NoContent> DeleteInvoiceAsync(int id, ApplicationDbContext dbContext)
    {
        List<ProductInvoice> productInvoices = await dbContext.ProductInvoices.Include(productInvoices => productInvoices.Product).Where(productInvoices => productInvoices.InvoiceId == id).AsNoTracking().ToListAsync();
        foreach (ProductInvoice productInvoice in productInvoices)
        {
            productInvoice.Product.Quantity += productInvoice.Quantity;
            dbContext.Products.Entry(productInvoice.Product).CurrentValues.SetValues(productInvoice.Product);
        }
        await dbContext.ProductInvoices.Where(pi => pi.InvoiceId == id).ExecuteDeleteAsync();
        await dbContext.Invoices.Where(invoice => invoice.Id == id).ExecuteDeleteAsync();
        await dbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
