using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int? WishListId { get; set; }
        public WishList? WishList { get; set; }

        public int? CartId { get; set; }
        public Cart? Cart { get; set; }

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<ApprovalJob> RequestedJobs { get; set; } = new List<ApprovalJob>();
        public ICollection<ApprovalJob> DecidedJobs { get; set; } = new List<ApprovalJob>();
    }
}
