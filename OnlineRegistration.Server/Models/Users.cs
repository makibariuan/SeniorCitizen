using System.ComponentModel.DataAnnotations;

namespace OnlineRegistration.Server.Models
{
    public class Users
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? ActiveToken { get; set; }

        // Email confirmation fields
        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailConfirmationToken { get; set; }
        public DateTime? EmailTokenExpiry { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public string? LoginOtp { get; set; }
        public DateTime? LoginOtpExpiry { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmployeeID { get; set; }
        public DateTime? BirthDate { get; set; }

    }
}
