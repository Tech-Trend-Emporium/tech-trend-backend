using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Validations
{
    public static class SessionValidator
    {
        // Entity validation messages
        public const string LogoutAtBeforeLoginAtErrorMessage = "The field LogoutAt cannot be before LoginAt field.";
    }
}
