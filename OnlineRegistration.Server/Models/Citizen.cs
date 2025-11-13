using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makatizen_app.Server.Models
{
    [Table("Citizen")]
    public class Citizen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CitizenType { get; set; }
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;
        public bool BiometricBypass { get; set; } = false;
        [Column(TypeName = "smalldatetime")]
        public DateTime BirthDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<BiometricDataEnrollment> BiometricEnrollments { get; set; } = new List<BiometricDataEnrollment>();
        public ICollection<BypassLog> BypassLogs { get; set; } = new List<BypassLog>();
    }
}