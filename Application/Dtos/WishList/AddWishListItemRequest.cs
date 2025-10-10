using Domain.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.WishList
{
    public class AddWishListItemRequest
    {
        [Required(ErrorMessage = WishListValidator.ProductIdRequiredMessage)]
        [Range(1, int.MaxValue, ErrorMessage = WishListValidator.ProductIdRangeMessage)]
        public int ProductId { get; set; }
    }
}
