using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace LTRegistrator.BLL.Services.Mappings.DbContext
{
    public class CustomFieldMap : IEntityTypeConfiguration<CustomField>
    {
        public void Configure(EntityTypeBuilder<CustomField> builder)
        {
            builder.ToTable("CustomFields");

            builder.HasKey(cf => cf.Id);

            builder.Property(cf => cf.Id).ValueGeneratedOnAdd();

            builder.HasMany(cf => cf.CustomValues).WithOne(cv => cv.CustomField).HasForeignKey(cv => cv.CustomFieldId);

            builder.HasMany(cf => cf.CustomFieldOptions).WithOne(cfo => cfo.CustomField)
                .HasForeignKey(cfo => cfo.CustomFieldId);
        }
    }
}
