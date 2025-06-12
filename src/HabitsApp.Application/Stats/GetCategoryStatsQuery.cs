using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Categories;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Stats;
public sealed record GetCategoryStatsQuery() : IRequest<Result<List<GetCategoryStatsResponse>>>;


public sealed record GetCategoryStatsResponse
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public int HabitCount { get; set; }
    public decimal SuccessRate { get; set; }

}

internal sealed class GetCategoryStatsQueryHandler(
    ICategoryRepository categoryRepository,
    IHabitRepository habitRepository,
    IHabitLogRepository habitLogRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<GetCategoryStatsQuery, Result<List<GetCategoryStatsResponse>>>
{
    public async Task<Result<List<GetCategoryStatsResponse>>> Handle(GetCategoryStatsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var userId = currentUserService.UserId;

        var logs =  habitLogRepository.GetAll()
            .Where(l => l.CreateUserId == userId).ToList();


        var groupedData =  (
    from habit in habitRepository.GetAll().Where(h => h.CreateUserId == userId)
    join category in categoryRepository.GetAll().Where(c => c.CreateUserId == userId)
    on habit.CategoryId equals category.Id into catGroup
    from category in catGroup.DefaultIfEmpty()
    select new
    {
        HabitId = habit.Id,
        CreatedAt = habit.CreatedAt,
        CategoryId = category != null ? category.Id : Guid.Empty,
        CategoryName = category != null ? category.Name : "Kategorisiz"
    }).ToList();

        var categoryHabits = groupedData
    .GroupBy(x => new { x.CategoryId, x.CategoryName })
    .Select(g => new
    {
        CategoryId = g.Key.CategoryId,
        CategoryName = g.Key.CategoryName,
        HabitCount = g.Count(),
        HabitIds = g.Select(h => h.HabitId).ToList(),
        HabitCreatedDates = g.ToDictionary(h => h.HabitId, h => h.CreatedAt)
    }).ToList();

        var results = new List<GetCategoryStatsResponse>();

        foreach (var categoryGroup in categoryHabits)
        {
            var categoryLogs = logs.Where(l => categoryGroup.HabitIds.Contains(l.HabitId)).ToList();

            decimal successRate = CalculateSuccessRate(categoryGroup.HabitIds, categoryGroup.HabitCreatedDates, categoryLogs);

            results.Add(new GetCategoryStatsResponse
            {
                CategoryId = categoryGroup.CategoryId,
                CategoryName = categoryGroup.CategoryName,
                HabitCount = categoryGroup.HabitCount,
                SuccessRate = successRate
            });
        }

        return await Task.FromResult( Result<List<GetCategoryStatsResponse>>.Success(results, "success"));
    }
    private decimal CalculateSuccessRate(List<Guid> habitIds, Dictionary<Guid, DateTime> habitCreatedDays, List<HabitLog> logs)
    {
        var today = DateTime.Today;
        var totalExpectedLog = 0;
        int actualLogs = 0;

        foreach (var habitId in habitIds)
        {
            var createdDate = habitCreatedDays[habitId].Date;
            var daysSinceCreated = (today - createdDate).Days + 1;

            if (daysSinceCreated > 0)
            {
                totalExpectedLog += daysSinceCreated;
            }

            // O habit için, createdDate'den bugüne kadar log sayısı
            actualLogs += logs.Count(l => l.HabitId == habitId && l.Date.Date >= createdDate && l.Date.Date <= today);
        }

        return totalExpectedLog > 0 ? Math.Round((decimal)actualLogs / totalExpectedLog * 100, 2) : 0;
    }
}
