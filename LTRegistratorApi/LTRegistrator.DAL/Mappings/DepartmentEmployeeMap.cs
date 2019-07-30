using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.DAL.Mappings
{
    public class DepartmentEmployeeMap: IEntityTypeConfiguration<DepartmentEmployee>
    {
        public void Configure(EntityTypeBuilder<DepartmentEmployee> builder)
        {
            builder.HasKey(de => new {de.DepartmentId, de.EmployeeId});

            builder.HasOne(de => de.Department).WithMany(d => d.DepartmentEmployee)
                .HasForeignKey(de => de.DepartmentId);

            builder.HasOne(de => de.Employee).WithMany(e => e.DepartmentEmployees).HasForeignKey(de => de.EmployeeId);
        }
    }
}
