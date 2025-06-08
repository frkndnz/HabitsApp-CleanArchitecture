using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions;
using HabitsApp.Domain.Habits;

namespace HabitsApp.Domain.Categories;
public sealed class Category:Entity
{
    public string Name { get; set; } = default!;
    public string? Emoji { get; set; }
    
    public ICollection<Habit>? Habits { get; set; }
}
