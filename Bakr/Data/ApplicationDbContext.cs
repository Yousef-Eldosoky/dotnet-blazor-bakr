using Bakr.Shared.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bakr.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<ProductInvoice> ProductInvoices => Set<ProductInvoice>();
}
