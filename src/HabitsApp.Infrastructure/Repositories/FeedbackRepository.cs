using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Feedbacks;
using HabitsApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Infrastructure.Repositories;
public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
{
    public FeedbackRepository(ApplicationDbContext context) : base(context)
    {
    }
}
