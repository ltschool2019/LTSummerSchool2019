using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.BLL.Services.Mappings.DbContext;
using LTRegistrator.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services
{
    public class LTRegistratorDbContext : IdentityDbContext<User, Role, Guid>
    {
        public LTRegistratorDbContext(DbContextOptions<LTRegistratorDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Leave> Leave { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployee { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmployeeMap());
            modelBuilder.ApplyConfiguration(new LeaveMap());
            modelBuilder.ApplyConfiguration(new ProjectEmployeeMap());
            modelBuilder.ApplyConfiguration(new ProjectMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new UserMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
