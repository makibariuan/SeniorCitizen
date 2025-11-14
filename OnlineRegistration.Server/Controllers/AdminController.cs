using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineRegistration.Server.Data;
using OnlineRegistration.Server.DTOs;
using OnlineRegistration.Server.Models;
namespace OnlineRegistration.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // --- Helper method for generating secure temporary passwords ---
        private (string plainPassword, string hashedPassword) GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            return (password, hash);
        }

        //        USER TYPE                 USER ROLE
        //        1 - System                1 - Super Admin
        //        2 - Kit                   2 - System User
        //                                  3 - Kit User


        [HttpGet("user-list/{userType}")]
        public async Task<IActionResult> GetSystemUsers(string userType)
        {
            userType = userType.ToLowerInvariant();

            int targetUserType;
            if (userType == "system")
            {
                targetUserType = 1;
            }
            else if (userType == "kit")
            {
                targetUserType = 2;
            }
            else
            {
                return BadRequest(new { message = "Invalid UserType. Must be 'system' or 'kit'." });
            }

            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.UserType == targetUserType)
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    UserType = u.UserType,
                    Username = u.Username,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Status = u.IsActive,
                    UserRole = u.UserRole,
                    RoleDesc = u.Role != null ? u.Role.RoleDesc : "N/A"
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateSystemUser([FromBody] UserCreateDto dto)
        {
            
            if (dto.UserType != 1 && dto.UserType != 2) 
            {
                return BadRequest(new { message = "Invalid UserType. Must be 1 (System) or 2 (Kit)." });
            }

            // Check for Username or Email conflict
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return Conflict(new { message = "Username already exists." });
            }

            // Validate Role ID
            var roleExists = await _context.UserRoles.AnyAsync(r => r.RoleId == dto.UserRole);
            if (!roleExists)
            {
                return BadRequest(new { message = $"Invalid UserRole ID: {dto.UserRole}." });
            }

            // Generate Credentials and Flags
            var (plainPassword, hashedPassword) = GenerateTemporaryPassword();

            //Create the new User
            var user = new Users
            {
                UserType = dto.UserType,
                UserRole = dto.UserRole,
                Username = dto.Username,
                PasswordHash = hashedPassword,
                IsActive = true,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MustResetPassword = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //Return Success Response 
            string userRoleName = await _context.UserRoles
                .Where(r => r.RoleId == user.UserRole)
                .Select(r => r.RoleDesc)
                .FirstOrDefaultAsync() ?? "Unknown Role";

            string userTypeText = user.UserType == 1 ? "System" : "Kit";

            return CreatedAtAction(nameof(CreateSystemUser), new { userType = userTypeText.ToLower() }, new
            {
                message = $"{userTypeText} User created successfully (Role: {userRoleName}).",
                userId = user.Id,
                username = dto.Username,
                initialPassword = plainPassword,
            });
        }

        //admin
        [HttpPost("reset-user-password")]
        public async Task<IActionResult> ResetUserPassword([FromBody] ResetUserPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (user != null)
            {
                return await ProcessPasswordChange(user);
            }

            return Unauthorized(new { message = "Invalid credentials." });
        }


        private async Task<IActionResult> ProcessPasswordChange<T>(T user) where T : class, IResettableUser
        {
            var generatedPasswordData = GenerateTemporaryPassword();
            string newPassword = generatedPasswordData.plainPassword;
            string hashedPassword = generatedPasswordData.hashedPassword;

            user.PasswordHash = hashedPassword;
            user.PasswordResetToken = newPassword;

            // Set an expiry for the token/password validity
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Set an expiration time

            // Force user to reset the password on next login
            user.MustResetPassword = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Password for user '{user.Username}' has been successfully reset.",
                //password = newPassword,
                resetToken = newPassword,
                userMustChangePassword = true
            });
        }


        [HttpPost("toggle-status")]
        public async Task<IActionResult> ToggleUserStatus([FromBody] ToggleUserStatusRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
            {
                return NotFound(new { message = $"User with ID {request.UserId} not found." });
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            // Proceed to the status toggle logic using the single found user
            return await ProcessStatusToggle(user, user.IsActive);
        }

        private async Task<IActionResult> ProcessStatusToggle<T>(T user, bool isActive) where T : class, IStatusUser
        {
            // Prevent locking out Super Admins (UserType 1) - Best practice security check
            if (user is Users systemUser && systemUser.UserType == 1 && !isActive)
            {
                return BadRequest(new { message = $"Cannot deactivate the primary Super Admin user ('{systemUser.Username}')" });
            }

            user.IsActive = isActive;
            await _context.SaveChangesAsync();

            string action = isActive ? "activated" : "deactivated (locked)";

            return Ok(new
            {
                message = $"User '{user.Username}' (ID: {user.Id}) has been successfully {action}.",
                isActive = user.IsActive
            });
        }




    }
}