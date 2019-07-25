using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.DAL
{
    public class LTRegistratorDbContext : IdentityDbContext<User, Role, Guid>
    {
        public LTRegistratorDbContext(DbContextOptions<LTRegistratorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Leave> Leave { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            //Configuring one-to-one relationships between AspNetUser and Employee 
            modelBuilder.Entity<User>()
                .HasOne(au => au.Employee)
                .WithOne(e => e.User);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(au => au.Employee)
                .HasForeignKey<User>(u => u.EmployeeId);
        }
    }
}
