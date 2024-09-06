using Microsoft.AspNetCore.Identity;

namespace DiscShop.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int DiscId { get; set; }
        public Disc Disc { get; set; }
        public int Quantity { get; set; }
        public string? UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
