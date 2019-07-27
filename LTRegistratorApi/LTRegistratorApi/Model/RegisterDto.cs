using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data required during registration.
/// </summary>
public class RegisterDto
{
    [Required]
    public string Name { get; set; }

    private string role;

    [Required]
    public string Role
    {
        get { return role; }
        set
        {
            if (value != "Employee" && value != "Manager" && value != "Administrator")
            {
                throw new ApplicationException("INVALID_ROLE");
            }
            role = value;
        }
    }

    [Required]
    [EmailAddress(ErrorMessage = "INCORRECT_EMAIL")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
    public string Password { get; set; }
}
