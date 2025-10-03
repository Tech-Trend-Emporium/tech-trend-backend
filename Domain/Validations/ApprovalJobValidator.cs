using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Constants
{
    public static class ApprovalJobValidator
    {
        // Entity validation messages
        public const string DecidedAtRequiresDecidedByErrorMessage = "The field DecidedAt requires DecidedBy field.";
        public const string DecidedAtCannotBeBeforeRequestedAtErrorMessage = "The field DecidedAt cannot be before RequestedAt field.";
        public const string DecidedByCannotBeSameAsRequestedByErrorMessage = "The field DecidedBy cannot be the same as RequestedBy field.";
    }
}
