using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LTRegistrator.BLL.Services.Mappings.DbContext
{
    public class TaskNoteMap : IEntityTypeConfiguration<TaskNote>
    {
        public void Configure(EntityTypeBuilder<TaskNote> builder)
        {
            builder.HasKey(tn => tn.Id);

            builder.Property(tn => tn.Id).ValueGeneratedOnAdd();

            builder.HasOne(tn => tn.Task).WithMany(t => t.TaskNotes).HasForeignKey(tn => tn.TaskId);

            builder.HasIndex(tn => new { tn.Day, tn.TaskId }).IsUnique();
        }
    }
}