using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.BLL.Services.Mappings.DbContext
{
    public class EmployeeMap : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.HasMany(e => e.ProjectEmployees).WithOne(pe => pe.Employee).HasForeignKey(pe => pe.EmployeeId);

            builder.HasOne(e => e.User).WithOne(au => au.Employee).HasForeignKey<User>(u => u.EmployeeId);

            builder.HasMany(e => e.Leaves).WithOne(l => l.Employee).HasForeignKey(l => l.EmployeeId);

            builder.HasOne(e => e.Manager).WithMany().HasForeignKey(e => e.ManagerId);
        }
    }
}
