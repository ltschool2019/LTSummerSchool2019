﻿using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.DAL.Mappings
{
    public class LeaveMap : IEntityTypeConfiguration<Leave>
    {
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id).HasDefaultValueSql("NEWID()");

            builder.HasOne(l => l.Employee).WithMany(e => e.Leaves).HasForeignKey(l => l.EmployeeId);
        }
    }
}
