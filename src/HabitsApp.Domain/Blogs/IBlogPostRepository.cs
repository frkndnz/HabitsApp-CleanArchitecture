using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Abstractions.Repositories;

namespace HabitsApp.Domain.Blogs;
public interface IBlogPostRepository:IGenericRepository<BlogPost>
{
}
