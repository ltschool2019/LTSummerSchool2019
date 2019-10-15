using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.BLL.Services.Mappings.DbContext
{
    public class CustomFieldOptionMap : IEntityTypeConfiguration<CustomFieldOption>
    {
        public void Configure(EntityTypeBuilder<CustomFieldOption> builder)
        {
            builder.ToTable("CustomFieldOptions");
        }
    }
}
