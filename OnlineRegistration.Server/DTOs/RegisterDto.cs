using System.ComponentModel.DataAnnotations;

namespace OnlineRegistration.Server.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employee ID is required")]
        public string EmployeeID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birth Date is required")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "User Type must be defined")]
        public int UserType { get; set; }

    }
}
