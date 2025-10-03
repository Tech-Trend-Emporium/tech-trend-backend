using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public static class CartValidator
    {
        // Entity validation messages
        public const string ProductAlreadyInCartErrorMessage = "The product is already in the cart.";
    }
}
