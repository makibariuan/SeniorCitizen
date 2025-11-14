using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRegistration.Server.Models
{
    // Ensure all interfaces (IResettableUser, IStatusUser) are correctly defined elsewhere.
    [Table("Users")]
    public class Users : IResettableUser, IStatusUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserType { get; set; } // 1=System, 2=Kit 

        // Foreign Key to UserRole table
        public int UserRole { get; set; } //1 = Super Admin, 2 = System User, 3 = Kit User

        [ForeignKey(nameof(UserRole))]
        public UserRole? Role { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        // Added properties from schema
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public bool MustResetPassword { get; set; } = true;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "nvarchar(max)")]
        public string? ActiveToken { get; set; }
        public int? OneTimePIN { get; set; }
        public bool? IsEmailConfirmed { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailTokenExpiry { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        [MaxLength(10)]
        public string? LoginOtp { get; set; }
        public DateTime? LoginOtpExpiry { get; set; }
    }
}