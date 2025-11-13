using System.ComponentModel.DataAnnotations;

namespace SeniorCitizen.Server.DTOs
{
    public class BiometricDataEnrollmentDto
    {
            public int Id { get; set; }
            [Required]
            public int PersonId { get; set; }
            public DateTime DateCapture { get; set; }
            public DateTime DateUpload { get; set; }
            public DateTime? DateActivate { get; set; }
            public int Status { get; set; } // Note: Status is non-nullable in this output DTO

            public string? Photo { get; set; }
            public string? Signature { get; set; }
            public string? LeftThumb { get; set; }
            public string? LeftIndex { get; set; }
            public string? LeftMiddle { get; set; }
            public string? LeftRing { get; set; }
            public string? LeftSmall { get; set; }
            public string? RightThumb { get; set; }
            public string? RightIndex { get; set; }
            public string? RightMiddle { get; set; }
            public string? RightRing { get; set; }
            public string? RightSmall { get; set; }
            public string? EyeLeft { get; set; }
            public string? EyeRight { get; set; }
            public string? BiometricLeft { get; set; }
            public string? BiometricRight { get; set; }

        //-----------------------------------------------------------------kulang
    }
}
