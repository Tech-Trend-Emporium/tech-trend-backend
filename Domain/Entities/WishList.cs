using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    [Index(nameof(UserId), IsUnique = true)]
    public class WishList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<WishListItem> Items { get; set; } = new List<WishListItem>();

        public void AddItem(int productId)
        {
            if (Items.Any(i => i.ProductId == productId))
                throw new InvalidOperationException("The product is already in the wish list.");

            Items.Add(new WishListItem { ProductId = productId, WishList = this });
        }

        public void RemoveItem(int productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null) return; 

            Items.Remove(item);
        }

        public void Clear() => Items.Clear();
    }

    [Index(nameof(WishListId), nameof(ProductId), IsUnique = true)]
    public class WishListItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
