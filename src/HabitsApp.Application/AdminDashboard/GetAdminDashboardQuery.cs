using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Domain.Blogs;
using HabitsApp.Domain.Feedbacks;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Shared;
using HabitsApp.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.AdminDashboard;
public sealed class GetAdminDashboardQuery:IRequest<Result<AdminDashboardQueryResponse>>
{
}

public class AdminDashboardQueryResponse
{
    public int TotalUsers { get; set; }
    public int TotalHabits { get; set; }
    public int TotalBlogs { get; set; }
    public int TotalFeedbacks { get; set; }

    public List<RecentUserDto> NewUsers { get; set; } = new();
    public List<RecentFeedbackDto> RecentFeedbacks { get; set; } = new();
    public List<RecentBlogDto> RecentBlogs { get; set; } = new();
}
public class RecentUserDto
{
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class RecentFeedbackDto
{
    public string? Subject { get; set; }
    public string? UserEmail { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class RecentBlogDto
{
    public string Title { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

internal sealed class GetAdminDashboardQueryHandler(
    UserManager<AppUser> userManager,
    IBlogPostRepository blogPostRepository,
    IFeedbackRepository feedbackRepository,
    IHabitRepository habitRepository
    ) : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardQueryResponse>>
{
    public async Task<Result<AdminDashboardQueryResponse>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var totalUsers = await userManager.Users.CountAsync();
        var totalBlogs=await blogPostRepository.CountAsync();
        var totalFeedbacks=await feedbackRepository.CountAsync();
        var totalHabit=await habitRepository.CountAsync();

        var newUsers = await userManager.Users.
            OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .Select(u => new RecentUserDto
            {
                Email = u.Email!,
                CreatedAt = u.CreatedAt,
            }).ToListAsync();

        var recentBlogs = await blogPostRepository.GetAll()
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .Select(b => new RecentBlogDto
            {
                CreatedAt = b.CreatedAt,
                Title = b.Title,
            }).ToListAsync();


        var recentFeedbacks = await (
                from feedback in feedbackRepository.GetAll()
                join user in userManager.Users
                on feedback.CreateUserId equals user.Id
                orderby feedback.CreatedAt descending
                select new RecentFeedbackDto
                {
                    CreatedAt = feedback.CreatedAt,
                    Subject = feedback.Subject,
                    UserEmail = user.Email,
                })
                .Take(5)
                .ToListAsync();

        var response = new AdminDashboardQueryResponse
        {
            TotalBlogs = totalBlogs,
            TotalFeedbacks = totalFeedbacks,
            TotalHabits = totalHabit,
            TotalUsers = totalUsers,
            RecentBlogs = recentBlogs,
            RecentFeedbacks = recentFeedbacks,
            NewUsers = newUsers,

        };
        return Result<AdminDashboardQueryResponse>.Success(response, "successfully taked admindashboard summary response!");

    }
}
