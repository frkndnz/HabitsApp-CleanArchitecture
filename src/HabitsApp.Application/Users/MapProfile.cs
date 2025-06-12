using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HabitsApp.Domain.Users;

namespace HabitsApp.Application.Users;
internal class MapProfile:Profile
{
    public  MapProfile()
    {
        CreateMap<UpdateUserCommand, AppUser>();
        CreateMap<AppUser,UpdateUserCommandResponse>();
    }
}
