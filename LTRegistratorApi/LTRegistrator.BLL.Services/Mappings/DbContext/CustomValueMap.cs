using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.BLL.Services.Mappings.DbContext
{
    public class CustomValueMap: IEntityTypeConfiguration<CustomValue>
    {
        public void Configure(EntityTypeBuilder<CustomValue> builder)
        {
            builder.ToTable("CustomValues");

            builder.HasKey(cv => cv.Id);

            builder.Property(cv => cv.Id).ValueGeneratedOnAdd();

            builder.HasOne(cv => cv.Task).WithMany(t => t.CustomValues).HasForeignKey(cv => cv.TaskId);
        }
    }
}
