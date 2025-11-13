using System.ComponentModel.DataAnnotations;

namespace OnlineRegistration.Server.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        //public bool ActiveToken { get; set; } = false;
    }

    //public class LoginResponse
    //{
    //    public string Token { get; set; } = string.Empty;
    //    public string Role { get; set; } = string.Empty;
    //    public string UserId { get; set; } = string.Empty;
    //    public bool MustResetPassword { get; set; }
    //}

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
    public class VerifyOtpDto
    {
        public string Username { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }


}
