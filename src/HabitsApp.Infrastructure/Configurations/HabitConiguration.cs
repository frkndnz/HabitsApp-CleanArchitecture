using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Habits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitsApp.Infrastructure.Configurations;
internal class HabitConiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> builder)
    {
        builder
           .HasMany(h => h.Logs)
           .WithOne()
           .HasForeignKey(hl => hl.HabitId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
