namespace DiscShop.Models
{
    public class Disc
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Plastic { get; set; }
        public string? Colour { get; set; }
        public int Weight { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Speed { get; set; }
        public int Glide { get; set; }
        public int Turn { get; set; }
        public int Fade { get; set; }
        public string? ImgUrl { get; set; }
    }
}
