using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.DAL.Mappings
{
    public class ValueMap: IEntityTypeConfiguration<Value>
    {
        public void Configure(EntityTypeBuilder<Value> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id).HasDefaultValueSql("NEWID()");
        }
    }
}
