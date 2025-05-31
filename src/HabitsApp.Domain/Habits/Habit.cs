using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;

namespace HabitsApp.Domain.Habits;
public sealed class Habit:Entity
{
    public string Name { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get;  set; }

    public ICollection<HabitLog>? Logs { get; set; }

    
}
