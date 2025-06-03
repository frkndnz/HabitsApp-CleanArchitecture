using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;

namespace HabitsApp.Domain.HabitLogs;
public sealed class HabitLog : Entity
{
    public DateTime Date { get; set; } = default!;
    public Guid HabitId { get; set; } = default!;

    public HabitLog()
    {
        Date = DateTime.UtcNow;
    }
}
