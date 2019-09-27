using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.Domain.Entities
{
    public class CustomFieldProjectMap : IEntityTypeConfiguration<CustomFieldProject>
    {
        public void Configure(EntityTypeBuilder<CustomFieldProject> builder)
        {
            builder.ToTable("CustomFieldProjects");

            builder.HasKey(cfp => new { cfp.CustomFieldId, cfp.ProjectId });

            builder.HasOne(cfp => cfp.CustomField).WithMany(cf => cf.CustomFieldProjects)
                .HasForeignKey(cfp => cfp.CustomFieldId);

            builder.HasOne(cfp => cfp.Project).WithMany(p => p.CustomFieldProjects).HasForeignKey(cfp => cfp.ProjectId);
        }
    }
}
