using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LTTimeRegistrator.Models
{
    /// <summary>
    /// Creating database entities and configuring relationships with the Fluent API.
    /// </summary>
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
          : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployee { get; set; }
        public DbSet<Project> Project { get; set; }

        public DbSet<DepartmentEmployee> DepartmentEmployee { get; set; }
        public DbSet<Department> Department { get; set; }

        public DbSet<Leave> Leave { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring many-to-many relationships between Project and Employee 
            modelBuilder.Entity<ProjectEmployee>()
                .HasKey(pe => new { pe.EmployeeId, pe.ProjectId });
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Employee)
                .WithMany(e => e.ProjectEmployee)
                .HasForeignKey(pe => pe.EmployeeId);
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Project)
                .WithMany(p => p.ProjectEmployee)
                .HasForeignKey(pe => pe.ProjectId);

            // Configuring many-to-many relationships between Department and Employee 
            modelBuilder.Entity<DepartmentEmployee>()
                .HasKey(pe => new { pe.EmployeeId, pe.DepartmentId });
            modelBuilder.Entity<DepartmentEmployee>()
                .HasOne(pe => pe.Department)
                .WithMany(e => e.DepartmentEmployee)
                .HasForeignKey(pe => pe.DepartmentId);
            modelBuilder.Entity<DepartmentEmployee>()
                .HasOne(pe => pe.Department)
                .WithMany(p => p.DepartmentEmployee)
                .HasForeignKey(pe => pe.DepartmentId);

            // Configuring one-to-one relationships between AspNetUser and Employee 
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(au => au.Employee)
                .WithOne(e => e.ApplicationUser);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithOne(au => au.Employee)
                .HasForeignKey<ApplicationUser>(u => u.EmployeeId);
        }
    }
}
