using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.BLL.Services.Mappings.DbContext
{
    public class ProjectEmployeeMap : IEntityTypeConfiguration<ProjectEmployee>
    {
        public void Configure(EntityTypeBuilder<ProjectEmployee> builder)
        {
            builder.ToTable("ProjectEmployees");

            builder.HasKey(pe => new { pe.EmployeeId, pe.ProjectId });

            builder.HasOne(pe => pe.Employee).WithMany(e => e.ProjectEmployees).HasForeignKey(pe => pe.EmployeeId);

            builder.HasOne(pe => pe.Project).WithMany(e => e.ProjectEmployees).HasForeignKey(pe => pe.ProjectId);

            builder.HasMany(pe => pe.Tasks).WithOne(t => t.ProjectEmployee).HasForeignKey(pe => new { pe.EmployeeId, pe.ProjectId });
        }
    }
}
