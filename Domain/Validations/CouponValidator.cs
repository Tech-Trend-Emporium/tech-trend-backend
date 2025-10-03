using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public class CouponValidator
    {
        // Entity validation messages
        public const string ValidToAfterValidFromErrorMessage = "The field ValidTo must be after ValidFrom field.";
    }
}
