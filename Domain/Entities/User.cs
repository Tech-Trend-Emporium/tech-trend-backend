using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(40)]
        public string Username { get; set; } = null!;

        [Required, MaxLength(256)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public Role Role { get; set; } = Role.SHOPPER;

        [Required]
        public bool IsActive { get; set; } = true;

        public int? RecoveryQuestionId { get; set; }
        public RecoveryQuestion? RecoveryQuestion { get; set; }

        [MaxLength(256)]
        public string? RecoveryAnswerHash { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(64)]
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");

        public WishList? WishList { get; set; }

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<ApprovalJob> RequestedJobs { get; set; } = new List<ApprovalJob>();
        public ICollection<ApprovalJob> DecidedJobs { get; set; } = new List<ApprovalJob>();
    }
}
