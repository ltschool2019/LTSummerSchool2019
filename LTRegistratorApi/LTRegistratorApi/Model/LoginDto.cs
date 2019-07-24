using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data required for authorization.
/// </summary>
public class LoginDto
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
