using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic; // Added for ICollection/List

namespace OnlineRegistration.Server.Models
{
    [Table("Citizen")]
    public class Citizen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CitizenType { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public bool BiometricBypass { get; set; } = false;

        [Required]
        [Column(TypeName = "smalldatetime")]
        public DateTime BirthDate { get; set; }
        [Required]
        [Column(TypeName = "datetime2(7)")]
        public DateTime CreatedAt { get; set; } 

        public virtual ICollection<BiometricDataEnrollment> BiometricEnrollments { get; set; } = new List<BiometricDataEnrollment>();
        public ICollection<BypassLog> BypassLogs { get; set; } = new List<BypassLog>();
    }
}