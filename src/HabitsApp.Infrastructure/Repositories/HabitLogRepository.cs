using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Infrastructure.Repositories;
internal sealed class HabitLogRepository : GenericRepository<HabitLog>, IHabitLogRepository
{
    public HabitLogRepository(ApplicationDbContext context) : base(context)
    {
    }
}
