using Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ApprovalJob
{
    public class DecideApprovalJobRequest : IValidatableObject
    {
        [Required(ErrorMessage = ApprovalJobValidator.ApproveRequiredMessage)]
        public bool Approve { get; set; }

        [StringLength(512, ErrorMessage = ApprovalJobValidator.ReasonMaxLengthMessage)]
        public string? Reason { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (Reason != null && Reason != Reason.Trim()) yield return new ValidationResult(ApprovalJobValidator.ReasonNoLeadingTrailingSpacesMessage, new[] { nameof(Reason) });

            if (!Approve) if (string.IsNullOrWhiteSpace(Reason)) yield return new ValidationResult(ApprovalJobValidator.RejectRequiresReasonMessage, new[] { nameof(Reason) });
            else if (Reason is not null && Reason.Trim().Length == 0) yield return new ValidationResult(ApprovalJobValidator.ReasonWhitespaceMessage, new[] { nameof(Reason) });
        }
    }
}
