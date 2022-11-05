
using Microsoft.EntityFrameworkCore;
using Eshop.Models;
namespace Eshop.Data
{
    public class EshopContext : DbContext
    {
        public EshopContext(DbContextOptions<EshopContext> options) : base(options){
        
        }
        public DbSet<Account> accounts { get; set; }

        public DbSet<Product> products { get; set; }

        public DbSet<Cart> carts { get; set; }

        public DbSet<ProductType> productTypes { get; set; }

        public DbSet<Invoice> invoices { get; set; }

        public DbSet<InvoiceDetail> invoiceDetails { get; set; }
    }
}
