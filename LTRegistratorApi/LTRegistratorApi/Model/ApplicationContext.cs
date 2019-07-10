using LTRegistratorApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LTTimeRegistrator.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationContext : IdentityDbContext<UserApplication>
    {
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

            modelBuilder.Entity<ProjectEmployee>()
                .HasKey(pe => new { pe.EmployeeId, pe.ProjectId});
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Employee)
                .WithMany(e => e.ProjectEmployee)
                .HasForeignKey(pe => pe.EmployeeId);
            modelBuilder.Entity<ProjectEmployee>()
                .HasOne(pe => pe.Project)
                .WithMany(p => p.ProjectEmployee)
                .HasForeignKey(pe => pe.ProjectId);

            modelBuilder.Entity<UserApplication>()
                .HasMany(ua => ua.Employees)
                .WithOne(e => e.UserApplication);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.UserApplication)
                .WithMany(ua => ua.Employees)
                .HasForeignKey(u => u.UserId)
                .HasPrincipalKey(a => a.Id);
                
        }
    }
}