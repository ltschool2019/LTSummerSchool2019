﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.DAL.Mappings
{
    public class ProjectMap : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");

            builder.HasMany(p => p.ProjectEmployees).WithOne(pe => pe.Project).HasForeignKey(pe => pe.ProjectId);
        }
    }
}
