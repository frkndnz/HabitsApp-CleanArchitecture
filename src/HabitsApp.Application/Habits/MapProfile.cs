using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HabitsApp.Domain.Habits;

namespace HabitsApp.Application.Habits;
internal class MapProfile:Profile
{
    public MapProfile()
    {
        CreateMap<HabitCreateCommand, Habit>();
        CreateMap<HabitUpdateCommand, Habit>();
        CreateMap<Habit, GetUserHabitsQueryResponse>()
            .ForMember(dest=>dest.IsCompletedToday,opt=>opt.Ignore());
    }
}
