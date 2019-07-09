using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LTTimeRegistrator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
          : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }

        public DbSet<Projects> Projects { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<ProjectsEmployee> ProjectsEmployee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectsEmployee>()
                .HasKey(pe => new { pe.EmployeeId, pe.ProjectId});
            modelBuilder.Entity<ProjectsEmployee>()
                .HasOne(pe => pe.Employee)
                .WithMany(e => e.ProjectsEmployee)
                .HasForeignKey(pe => pe.EmployeeId);
            modelBuilder.Entity<ProjectsEmployee>()
                .HasOne(pe => pe.Projects)
                .WithMany(p => p.ProjectsEmployee)
                .HasForeignKey(pe => pe.ProjectId);
        }
    }
}