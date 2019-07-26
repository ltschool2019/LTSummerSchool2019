using System;
using System.ComponentModel.DataAnnotations;
using LTRegistratorApi.Model;

/// <summary>
/// Data required during registration.
/// </summary>
public class RegisterDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string SecondName { get; set; }
    private RoleType role;
    [Required]
    public RoleType Role
    {
        get { return role; }
        set
        {
            if (value != RoleType.Employee && value != RoleType.Manager && value != RoleType.Administrator)
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
