using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.BLL.Services.Mappings.DbContext;
using LTRegistrator.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services
{
    public class LTRegistratorDbContext : IdentityDbContext<User>
    {
        public LTRegistratorDbContext(DbContextOptions<LTRegistratorDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Leave> Leave { get; set; }
        public DbSet<ProjectEmployee> ProjectEmployee { get; set; }
        public DbSet<Task> Task { get; set; }
        public DbSet<TaskNote> TaskNote { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<CustomFieldProject> CustomFieldProjects { get; set; }
        public DbSet<CustomValue> CustomValues { get; set; }
        public DbSet<CustomFieldOption> CustomFieldOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EmployeeMap());
            modelBuilder.ApplyConfiguration(new LeaveMap());
            modelBuilder.ApplyConfiguration(new ProjectEmployeeMap());
            modelBuilder.ApplyConfiguration(new ProjectMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new TaskMap());
            modelBuilder.ApplyConfiguration(new TaskNoteMap());
            modelBuilder.ApplyConfiguration(new CustomFieldMap());
            modelBuilder.ApplyConfiguration(new CustomFieldProjectMap());
            modelBuilder.ApplyConfiguration(new CustomValueMap());
            modelBuilder.ApplyConfiguration(new CustomFieldOptionMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
