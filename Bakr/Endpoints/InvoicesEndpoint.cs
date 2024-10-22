using System.Security.Claims;
using Bakr.Data;
using Bakr.Dtos;
using Bakr.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bakr.Endpoints;

public static class InvoicesEndpoint
{
    const string getInvoiceEndpointName = "GetInvoice";

    public static RouteGroupBuilder MapInvoiceEndpoint(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("invoices").WithParameterValidation().RequireAuthorization();

        group.MapGet("/", async (BakrDbContext dbContext) =>
        {
            List<Invoice> invoices = await dbContext.Invoices.Include(p => p.ProductInvoices).AsNoTracking().ToListAsync();
            return Results.Ok(invoices);
        });

        group.MapPost("/", async (CreateInvoiceDto newInvoice, BakrDbContext dbContext, ClaimsPrincipal claims) =>
        {
            string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (newInvoice.ProductQuantity.Count != newInvoice.ProductsId.Count || newInvoice.ProductQuantity.Count == 0)
            {
                return Results.BadRequest("The quantity array not equal to the product array or either of them is empty.");
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
                if (newInvoice.ProductQuantity[i] < 1) return Results.BadRequest("The quantity of a product can not be less than 1.");
                Product? product = await dbContext.Products.FindAsync(newInvoice.ProductsId[i]);
                if (product is null) return Results.NotFound("One or more Item is not Found in the database.");
                if (newInvoice.ProductQuantity[i] > product.Quantity) return Results.BadRequest($"Quantity of {product.Name} is not enough.");
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
            return Results.CreatedAtRoute(getInvoiceEndpointName, new { id = invoice.Id }, invoice);
        });

        group.MapGet("/{id}", async (int id, BakrDbContext dbContext) =>
        {
            Invoice? invoice = await dbContext.Invoices.Include(i => i.ProductInvoices).FirstOrDefaultAsync(i => i.Id == id);
            if (invoice is null) return Results.NotFound();
            InvoiceDto invoiceDto = new(
                id,
                invoice.Name,
                invoice.ProductInvoices,
                invoice.DateTime,
                invoice.DiscountInPrice
            );
            return Results.Ok(invoiceDto);
        }).WithName(getInvoiceEndpointName);

        // Unfinshed function
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

        group.MapDelete("/{id}", async (int id, BakrDbContext dbContext) =>
        {
            List<ProductInvoice> productInvoices = await dbContext.ProductInvoices.Include(productInvoices => productInvoices.Product).Where(productInvoices => productInvoices.InvoiceId == id).AsNoTracking().ToListAsync();
            foreach (ProductInvoice productInvoice in productInvoices)
            {
                productInvoice.Product.Quantity += productInvoice.Quantity;
                dbContext.Products.Entry(productInvoice.Product).CurrentValues.SetValues(productInvoice.Product);
            }
            await dbContext.ProductInvoices.Where(ProductInvoice => ProductInvoice.InvoiceId == id).ExecuteDeleteAsync();
            await dbContext.Invoices.Where(invoice => invoice.Id == id).ExecuteDeleteAsync();
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization("AdminPolicy");;
        return group;
    }
}
