using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineRegistration.Server.DTOs
{
    public class CitizenVerificationDto
    {
        [Required]
        public int PersonId { get; set; } // Citizen ID being verified

        [Required]
        [MaxLength(100)]
        public string BiometricDeviceID { get; set; } = string.Empty; // Device used for verification

        public int Mode { get; set; } // e.g., 0=Fingerprint, 1=Iris
        public int EventType { get; set; } // e.g., 1=Check-In, 2=Check-Out
        public DateTime DateLog { get; set; }

        // This holds the actual data template/hash returned by the device for logging
        [Column(TypeName = "nvarchar(max)")]
        public string? VerificationData { get; set; }
    }
}