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

        // DTO validation messages
        public const string ApproveRequiredMessage = "The field Approve is required.";
        public const string ReasonMaxLengthMessage = "The field Reason must be a maximum length of 512 characters.";
        public const string ReasonNoLeadingTrailingSpacesMessage = "The field Reason cannot contain leading or trailing spaces.";
        public const string RejectRequiresReasonMessage = "The field Reason is required when Approve is false.";
        public const string ReasonWhitespaceMessage = "The field Reason cannot be only whitespace.";
        public const string PayloadMaxLengthMessage = "The field Payload must be a maximum length of 8000 characters.";
        public const string ApprovalJobTypeRequiredMessage = "The field Type is required.";
        public const string OperationRequiredMessage = "The field Operation is required.";
        public const string TypeInvalidMessage = "The field Type is invalid.";
        public const string OperationInvalidMessage = "The field Operation is invalid.";
        public const string TargetIdPositiveMessage = "The field TargetId must be a positive integer.";
    }
}
