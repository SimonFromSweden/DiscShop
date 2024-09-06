namespace DiscShop.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; } 
        public int? Quantity { get; set; }
        public int TotalPrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? UserId { get; set; }
    }
}


