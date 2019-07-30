using System;
using System.ComponentModel.DataAnnotations;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
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
    private RoleType _role;
    [Required]
    public RoleType Role
    {
        get { return _role; }
        set
        {
            if (value != RoleType.Employee && value != RoleType.Manager && value != RoleType.Administrator)
            {
                throw new ApplicationException("INVALID_ROLE");
            }
            _role = value;
        }
    }

    [Required]
    [EmailAddress(ErrorMessage = "INCORRECT_EMAIL")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
    public string Password { get; set; }
}
