using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HabitsApp.Domain.Blogs;

namespace HabitsApp.Application.BlogPosts;
public class MapProfile:Profile
{
    public MapProfile() 
    {
        CreateMap<BlogPostUpdateCommand, BlogPost>()
            .ForMember(dest=>dest.ImageUrl,opt=>opt.Ignore());
    }
}
