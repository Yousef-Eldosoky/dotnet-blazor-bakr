using Bakr.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bakr.Data;

public class BakrDbContext(DbContextOptions<BakrDbContext> options) : DbContext(options )
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<ProductInvoice> ProductInvoices => Set<ProductInvoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Genre>().HasData(
            new Genre {
                Id = 1,
                Name = "Cross fit"
            },
            new {
                Id = 2,
                Name = "اوزان"
            },
            new {
                Id = 3,
                Name = "Fit"
            },
            new {
                Id = 4,
                Name = "Boxing"
            },
            new {
                Id = 5,
                Name = "حديد"
            }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product {
                Id = 1,
                Name = "Bar",
                Price = 99.99M,
                Description = "",
                Picture = "",
                Quantity = 5,
                // GenreId = 1,
            },
            new  {
                Id = 2,
                Name = "Heavy lefting",
                Price = 499.99M,
                Description = "",
                Picture = "",
                Quantity = 50,
                // GenreId = 1,
            }
        );
    }
}
