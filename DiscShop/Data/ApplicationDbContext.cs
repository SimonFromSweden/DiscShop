using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DiscShop.Models;

namespace DiscShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<Disc> Disc { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        

    }

}
