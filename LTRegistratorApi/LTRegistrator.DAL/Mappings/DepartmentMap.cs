using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.DAL.Mappings
{
    public class DepartmentMap : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");

            builder.HasMany(d => d.DepartmentEmployee).WithOne(de => de.Department)
                .HasForeignKey(de => de.DepartmentId);
        }
    }
}
