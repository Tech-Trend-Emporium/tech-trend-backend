namespace Application.Dtos.WishList
{
    public class WishListItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
