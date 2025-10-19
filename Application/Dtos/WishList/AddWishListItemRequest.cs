using Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace General.Dto.WishList
{
    public class AddWishListItemRequest
    {
        [Required(ErrorMessage = WishListValidator.ProductIdRequiredMessage)]
        [Range(1, int.MaxValue, ErrorMessage = WishListValidator.ProductIdRangeMessage)]
        public int ProductId { get; set; }
    }
}
