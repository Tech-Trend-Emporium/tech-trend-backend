using Domain.Constants;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Dtos.ApprovalJob
{
    public class SubmitApprovalJobRequest : IValidatableObject
    {
        [Required(ErrorMessage = ApprovalJobValidator.ApprovalJobTypeRequiredMessage)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ApprovalJobType Type { get; set; }

        [Required(ErrorMessage = ApprovalJobValidator.OperationRequiredMessage)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Operation Operation { get; set; }
        
        public int? TargetId { get; set; }

        [StringLength(512, ErrorMessage = ApprovalJobValidator.PayloadMaxLengthMessage)]
        public object? Payload { get; set; }

        [StringLength(512, ErrorMessage = ApprovalJobValidator.ReasonMaxLengthMessage)]
        public string? Reason { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext _)
        {
            if (!Enum.IsDefined(typeof(ApprovalJobType), Type)) yield return new ValidationResult(ApprovalJobValidator.TypeInvalidMessage, new[] { nameof(Type) });
            if (!Enum.IsDefined(typeof(Operation), Operation)) yield return new ValidationResult(ApprovalJobValidator.OperationInvalidMessage, new[] { nameof(Operation) });
            if (TargetId.HasValue && TargetId.Value <= 0) yield return new ValidationResult(ApprovalJobValidator.TargetIdPositiveMessage, new[] { nameof(TargetId) });
            if (Reason != null && Reason != Reason.Trim()) yield return new ValidationResult(ApprovalJobValidator.ReasonNoLeadingTrailingSpacesMessage, new[] { nameof(Reason) });
            if (Reason is not null && Reason.Trim().Length == 0) yield return new ValidationResult(ApprovalJobValidator.ReasonWhitespaceMessage, new[] { nameof(Reason) });
        }
    }
}
