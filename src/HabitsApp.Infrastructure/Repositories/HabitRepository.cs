using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Habits;
using HabitsApp.Infrastructure.Context;

namespace HabitsApp.Infrastructure.Repositories;
internal sealed class HabitRepository:GenericRepository<Habit>,IHabitRepository
{
    public HabitRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        
    }
}
