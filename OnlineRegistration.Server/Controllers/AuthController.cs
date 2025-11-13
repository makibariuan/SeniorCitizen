using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineRegistration.Server.Data;
using OnlineRegistration.Server.DTOs;
using OnlineRegistration.Server.Models;
using OnlineRegistration.Server.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MimeKit;
using MailKit.Net.Smtp;


namespace OnlineRegistration.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<UsersEmployee> _hasher;
        private readonly IConfiguration _config;
        private readonly IEmailQueue _emailQueue;

        public AuthController(AppDbContext context, IPasswordHasher<UsersEmployee> hasher, IConfiguration config, IEmailQueue emailQueue)
        {
            _context = context;
            _hasher = hasher;
            _config = config;
            _emailQueue = emailQueue;
        }

        //        USER TYPE                 USER ROLE
        //        1 - System                1 - Super Admin
        //        2 - Kit                   2 - System User
        //                                  3 - Kit User


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Role) 
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (user == null || !user.IsActive)
            {
                return Unauthorized(new { message = "Invalid credentials or account inactive." });
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }
            var roleName = user.Role?.RoleDesc ?? "Unknown Role";

            //Handle MustResetPassword state
            if (user.MustResetPassword)
            {
                return Ok(new LoginResponse
                {
                    Token = "",
                    UserRole = roleName,
                    MustResetPassword = true
                });
            }

            //Generate the token
            var token = GenerateJwtToken(user.Id.ToString(), user.UserType.ToString(), roleName);
            user.ActiveToken = token;
            await _context.SaveChangesAsync();

            return Ok(new LoginResponse
            {
                Message = "Login successful.",
                Token = token,
                UserRole = roleName,
                MustResetPassword = false
            });
        }

        //private IActionResult AuthenticateUser(int userId, string storedHash, int userType, bool mustResetPassword, string providedPassword, string userSource)
        //{
        //    if (!BCrypt.Net.BCrypt.Verify(providedPassword, storedHash))
        //    {
        //        return Unauthorized(new { message = "Invalid credentials." });
        //    }

        //    if (mustResetPassword)
        //    {

        //        return Ok(new LoginResponse
        //        {
        //            Token = "",
        //            UserRole = GetRoleName(userType),
        //            MustResetPassword = true
        //        });
        //    }

        //    // Generate the token
        //    var token = GenerateJwtToken(userId.ToString(), userType.ToString(), GetRoleName(userType));

        //    return Ok(new LoginResponse
        //    {
        //        Message = "Login successful.",
        //        Token = token,
        //        UserRole = GetRoleName(userType),
        //        //UserId = userId.ToString(),
        //        MustResetPassword = false
        //    });
        //}

        private string GenerateJwtToken(string userId, string userType, string roleName)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
            if (!int.TryParse(jwtSection["ExpireMinutes"], out int expiryMinutes))
            {
                expiryMinutes = 60; // Default to 60 minutes if parsing fails
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("UserType", userType)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //// ---------------- REGISTER ----------------
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    dto.Email = dto.Email.Trim().ToLower();

        //    if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
        //        return BadRequest(new { message = "Email already registered" });

        //    var user = new Users
        //    {
        //        Username = dto.Username.Trim(),
        //        Email = dto.Email,
        //        PasswordHash = _hasher.HashPassword(null!, dto.Password),
        //        EmailConfirmationToken = Guid.NewGuid().ToString("N"),
        //        EmailTokenExpiry = DateTime.UtcNow.AddHours(24),
        //        FirstName = dto.FirstName?.Trim(),
        //        LastName = dto.LastName?.Trim(),
        //        BirthDate = dto.BirthDate,
        //        EmployeeID = dto.EmployeeID?.Trim(),
        //        UserType = !string.IsNullOrWhiteSpace(dto.EmployeeID) ? 1 : 0,
        //    };

        //    _db.Users.Add(user);
        //    await _db.SaveChangesAsync();

        //    var frontendUrl = _config["FrontendUrl"];
        //    var confirmUrl = $"{frontendUrl}/confirm-email?token={user.EmailConfirmationToken}&email={user.Email}";

        //    _emailQueue.QueueEmail(new EmailMessage
        //    {
        //        To = user.Email,
        //        Subject = "Confirm your account",
        //        Body = $"<p>Hello {user.FirstName ?? user.Username},</p>" +
        //               $"<p>Please confirm your account by clicking the link below:</p>" +
        //               $"<p><a href='{confirmUrl}'>Confirm Email</a></p>"
        //    });

        //    return Ok(new { message = "Registration successful. Please check your email to confirm your account." });
        //}

        //[HttpGet("confirm-email")]
        //public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
        //{
        //    var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.EmailConfirmationToken == token);

        //    if (user == null)
        //        return BadRequest(new { message = "Invalid token or email" });

        //    if (user.EmailTokenExpiry < DateTime.UtcNow)
        //        return BadRequest(new { message = "Token has expired" });

        //    user.IsEmailConfirmed = true;
        //    user.EmailConfirmationToken = null;
        //    user.EmailTokenExpiry = null;

        //    await _db.SaveChangesAsync();

        //    return Ok(new { message = "Email confirmed successfully. You can now log in." });
        //}

        //// ---------------- RESEND CONFIRMATION ----------------
        //[HttpPost("resend-confirmation")]
        //public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationDto dto)
        //{
        //    var email = dto.Email.Trim().ToLower();
        //    var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        //    if (user == null)
        //        return BadRequest(new { message = "Email not found" });

        //    if (user.IsEmailConfirmed)
        //        return BadRequest(new { message = "Email is already confirmed" });

        //    // Generate new token + extend expiry
        //    user.EmailConfirmationToken = Guid.NewGuid().ToString("N");
        //    user.EmailTokenExpiry = DateTime.UtcNow.AddHours(24);

        //    await _db.SaveChangesAsync();

        //    var frontendUrl = _config["FrontendUrl"]; // e.g. https://yourdomain.com
        //    var confirmUrl = $"{frontendUrl}/confirm-email?token={user.EmailConfirmationToken}&email={user.Email}";

        //    //await _emailService.SendEmailAsync(
        //    //    user.Email,
        //    //    "Resend Confirmation Email",
        //    //    $"<p>Hello {user.Username},</p><p>Please confirm your account by clicking the link below:</p><p><a href='{confirmUrl}'>Confirm Email</a></p>"
        //    //);

        //    _emailQueue.QueueEmail(new EmailMessage
        //    {
        //        To = user.Email,
        //        Subject = "Resend Confirmation Email",
        //        Body = $"<p>Hello {user.Username},</p>" +
        //           $"<p>Please confirm your account by clicking the link below:</p>" +
        //           $"<p><a href='{confirmUrl}'>Confirm Email</a></p>"
        //    });

        //    return Ok(new { message = "A new confirmation email has been sent. Please check your inbox." });
        //}

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var email = dto.Email.Trim().ToLower();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Ok(new { message = "If this email is registered, a reset link has been sent." });
            // do not reveal if email exists

            // Generate reset token + expiry
            user.PasswordResetToken = Guid.NewGuid().ToString("N");
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _context.SaveChangesAsync();

            var frontendUrl = _config["FrontendUrl"];
            var resetUrl = $"{frontendUrl}/reset-password?token={user.PasswordResetToken}&email={user.Email}";

            //await _emailService.SendEmailAsync(
            //    user.Email,
            //    "Password Reset Request",
            //    $"<p>Hello {user.Username},</p><p>Reset your password by clicking the link below:</p><p><a href='{resetUrl}'>Reset Password</a></p>"
            //);

            _emailQueue.QueueEmail(new EmailMessage
            {
                To = user.Email,
                Subject = "Password Reset Request",
                Body = $"<p>Hello {user.Username},</p>" +
                   $"<p>Reset your password by clicking the link below:</p>" +
                   $"<p><a href='{resetUrl}'>Reset Password</a></p>"
            });

            return Ok(new { message = "If this email is registered, a reset link has been sent." });
        }

        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        //{
        //    var user = await _db.Users.FirstOrDefaultAsync(u =>
        //        u.Email == dto.Email && u.PasswordResetToken == dto.Token);

        //    if (user == null)
        //        return BadRequest(new { message = "Invalid token or email" });

        //    if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
        //        return BadRequest(new { message = "Token has expired" });

        //    user.PasswordHash = _hasher.HashPassword(user, dto.NewPassword);
        //    user.PasswordResetToken = null;
        //    user.PasswordResetTokenExpiry = null;

        //    await _db.SaveChangesAsync();

        //    return Ok(new { message = "Password has been reset successfully." });
        //}


        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginDto dto)
        //{
        //    var user = await _db.EmployeeUsers.FirstOrDefaultAsync(u => u.Username == dto.Username);
        //    if (user == null)
        //        return BadRequest(new { message = "Invalid credentials" });

        //    var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        //    if (result == PasswordVerificationResult.Failed)
        //        return BadRequest(new { message = "Invalid credentials" });

        //    if (!user.IsEmailConfirmed)
        //        return BadRequest(new { message = "Please confirm your email before logging in." });

        //    // Generate OTP (6 digits)
        //    var otp = new Random().Next(100000, 999999).ToString();
        //    user.LoginOtp = otp;
        //    user.LoginOtpExpiry = DateTime.UtcNow.AddMinutes(5); // OTP valid for 5 minutes
        //    await _db.SaveChangesAsync();

        //    // Send OTP via email
        //    //var subject = "Your Login OTP";
        //    //var body = $"<p>Hello {user.Username},</p>" +
        //    //           $"<p>Your OTP for login is: <strong>{otp}</strong></p>" +
        //    //           "<p>It expires in 5 minutes.</p>";

        //    //await _emailService.SendEmailAsync(user.Email, subject, body);

        //    _emailQueue.QueueEmail(new EmailMessage
        //    {
        //        To = user.Email,
        //        Subject = "Your Login OTP",
        //        Body = $"<p>Hello {user.Username},</p>" +
        //               $"<p>Your OTP for login is: <strong>{otp}</strong></p>" +
        //               $"<p>It expires in 5 minutes.</p>"
        //    });

        //    // Return message indicating OTP sent
        //    return Ok(new { message = "OTP sent to your email. Please enter it to complete login." });
        //}

        //[HttpPost("verify-otp")]
        //public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        //{
        //    var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        //    if (user == null)
        //        return Unauthorized(new { message = "Invalid OTP" });

        //    if (user.LoginOtpExpiry < DateTime.UtcNow || user.LoginOtp != dto.Otp)
        //        return Unauthorized(new { message = "Invalid or expired OTP" });

        //    // OTP is valid → generate JWT
        //    var token = GenerateJwtToken(user);

        //    // Clear OTP and save active token
        //    user.LoginOtp = null;
        //    user.LoginOtpExpiry = null;
        //    user.ActiveToken = token;
        //    await _db.SaveChangesAsync();

        //    return Ok(new { token });
        //}

        // ---------------- LOGOUT ----------------
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
                return Unauthorized();

            user.ActiveToken = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logged out" });
        }
    }
}
