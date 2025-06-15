using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Blogs;
using HabitsApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Infrastructure.Repositories;
internal sealed class BlogPostRepository : GenericRepository<BlogPost>, IBlogPostRepository
{
    public BlogPostRepository(ApplicationDbContext context) : base(context)
    {
    }
}
