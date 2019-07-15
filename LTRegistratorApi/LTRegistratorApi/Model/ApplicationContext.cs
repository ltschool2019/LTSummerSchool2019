using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LTTimeRegistrator.Models
{
    /// <summary>
    ///   Creating database entities and configuring relationships with the Fluent API.
    /// </summary>
    public class ApplicationContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
          : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }


    }
}