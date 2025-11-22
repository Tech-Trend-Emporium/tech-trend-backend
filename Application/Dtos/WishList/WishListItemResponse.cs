namespace Application.Dtos.WishList
{
    public class WishListItemResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
