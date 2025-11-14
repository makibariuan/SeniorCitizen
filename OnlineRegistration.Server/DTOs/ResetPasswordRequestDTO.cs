using System.ComponentModel.DataAnnotations;

public class ResetUserPasswordRequest
{
    [Required]
    public string Username { get; set; }
}

public class ResetAuthPasswordRequest
{
    [Required]
    public string Username { get; set; }

    // The token the Admin manually obtained (e.g., from the user's email or database lookup)
    [Required]
    public string ResetToken { get; set; }

    // The new desired password (input by the Admin on behalf of the user or instructed by the user)
    [Required]
    [MinLength(8, ErrorMessage = "New password must be at least 8 characters.")]
    public string NewPassword { get; set; }
}