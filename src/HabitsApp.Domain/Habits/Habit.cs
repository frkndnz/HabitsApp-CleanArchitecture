using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;
using HabitsApp.Domain.HabitLogs;

namespace HabitsApp.Domain.Habits;
public sealed class Habit:Entity
{
    public string Name { get; set; } = default!;
    public string? Description { get;  set; }
    public string Color { get; set; } = default!;

    public ICollection<HabitLog>? Logs { get; set; }

    
}
