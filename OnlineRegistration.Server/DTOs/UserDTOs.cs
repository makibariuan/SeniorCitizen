using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OnlineRegistration.Server.DTOs
{
    // =========================================================
    //               USER CREATION/UPDATE DTOs
    // =========================================================

    // DTO for creating a new user (System or Kit), used by AdminController.
    // FirstName and LastName are explicitly removed as requested.


    // DTO for creating a new user (System or Kit), used by AdminController.
    public class UserCreateDto
    {
        [Required]
        public int UserType { get; set; } 
        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int UserRole { get; set; }


    }

    // DTO for returning System User data
    public class UserReadDto
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool Status { get; set; }
        public int UserRole { get; set; }
        public string RoleDesc { get; set; } = string.Empty;

    }

    // DTO for returning Kit User data
    //public class UserKitReadDto
    //{
    //    public int Id { get; set; }
    //    public int UserType { get; set; }
    //    public string Username { get; set; } = string.Empty;
    //    public bool MustResetPassword { get; set; }
    //    public bool Status { get; set; }
    //}

}