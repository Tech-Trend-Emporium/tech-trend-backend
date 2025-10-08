using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public static class WishListValidator
    {
        // Entity validation messages
        public const string ProductAlreadyInWishListErrorMessage = "The product is already in the wish list.";

        // DTO validation messages
        public const string ProductIdRequiredMessage = "Product ID is required.";
        public const string ProductIdRangeMessage = "Product ID must be greater than 0.";
    }
}
