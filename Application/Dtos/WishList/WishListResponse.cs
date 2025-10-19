using Application.Dtos.WishList;

namespace General.Dto.WishList
{
    public class WishListResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public IReadOnlyList<WishListItemResponse> Items { get; set; } = new List<WishListItemResponse>(); 
    }
}
