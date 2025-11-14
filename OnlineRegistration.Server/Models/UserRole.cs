using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace OnlineRegistration.Server.Models
{
    [Table("UserRole")]
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Assuming RoleId is manually managed
        public int RoleId { get; set; } // PK, int, not null

        [Required]
        [MaxLength(100)]
        public string RoleDesc { get; set; } = string.Empty; // nvarchar(100), not null

        public ICollection<Users> Users { get; set; } = new List<Users>();
    }
}