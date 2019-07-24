using Microsoft.AspNetCore.Identity;

namespace LTRegistratorApi.Model
{
    /// <summary>
    /// Links IdentityUser to Employee entity.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public virtual Employee Employee { get; set; }
        public int EmployeeId { get; set; }
    }
}
