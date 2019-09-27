using System;
using System.Collections.Generic;
using System.Text;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.BLL.Services.Mappings.DbContext
{
    public class TaskMap : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.ToTable("Tasks");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.HasOne(t => t.ProjectEmployee).WithMany(pe => pe.Tasks).HasForeignKey(t => new { t.EmployeeId, t.ProjectId });

            builder.HasMany(t => t.TaskNotes).WithOne(tn => tn.Task).HasForeignKey(t => t.TaskId);
        }
    }
}
