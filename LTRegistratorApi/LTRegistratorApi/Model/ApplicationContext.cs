using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LTTimeRegistrator.Models
{
    /// <summary>
    ///   Creating database entities and configuring relationships with the Fluent API.
    /// </summary>
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
          : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configuring many-to-many relationships between Project and Employee 
            modelBuilder.Entity<ProjectEmployee>()
                .HasKey(pe => new { pe.EmployeeId, pe.ProjectId});
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Employee)
                .WithMany(e => e.ProjectEmployee)
                .HasForeignKey(pe => pe.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Project)
                .WithMany(p => p.ProjectEmployee)
                .HasForeignKey(pe => pe.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configuring one-to-one relationships between AspNetUser and Employee 
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(au => au.Employee)
                .WithOne(e => e.ApplicationUser);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithOne(au => au.Employee)
                .HasForeignKey<Employee>(u => u.UserId);

            //Configuring one-to-many relationships between Employee and Project
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Employee)
                .WithMany(e => e.Project)          
                .HasForeignKey(u => u.ManagerId);


        }
    }
}
