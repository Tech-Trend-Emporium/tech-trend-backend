using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Index(nameof(UserId), IsUnique = true)]
    public class WishList
    {
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<WishListItem> Items { get; set; } = new List<WishListItem>();
    }

    [Index(nameof(WishListId), nameof(ProductId), IsUnique = true)]
    public class WishListItem
    {
        public int Id { get; set; }

        [Required]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int WishListId { get; set; }
        public WishList WishList { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
