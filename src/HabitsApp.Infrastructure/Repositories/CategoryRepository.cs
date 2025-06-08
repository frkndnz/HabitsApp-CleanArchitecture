using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Categories;
using HabitsApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Infrastructure.Repositories;
internal sealed class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}
