using Application.Dtos.WishList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
