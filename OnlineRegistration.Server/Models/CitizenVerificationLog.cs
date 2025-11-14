using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace OnlineRegistration.Server.Models
{
    [Table("CitizenVerificationLog")]
    public class CitizenVerificationLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PersonId { get; set; } // Foreign Key to Citizen
        public Citizen? Citizen { get; set; } // Navigation Property

        [Required]
        [MaxLength(100)]
        public string BiometricDeviceID { get; set; } = string.Empty; // FK to BiometricDevice

        [Required]
        public int Mode { get; set; }

        [Required]
        public int EventType { get; set; }

        [Required]
        public DateTime DateLog { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? VerificationData { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}