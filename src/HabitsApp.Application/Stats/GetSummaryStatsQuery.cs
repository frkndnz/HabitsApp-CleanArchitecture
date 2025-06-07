using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitsApp.Application.Services;
using HabitsApp.Domain.Abstractions.Repositories;
using HabitsApp.Domain.HabitLogs;
using HabitsApp.Domain.Habits;
using HabitsApp.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HabitsApp.Application.Stats;
public sealed record GetSummaryStatsQuery() : IRequest<Result<GetSummaryStatsQueryResponse>>;


public class GetSummaryStatsQueryResponse
{
    public int TotalHabits { get; set; }
    public int ActiveHabits { get; set; }
    public string LongestStreakHabitName { get; set; } = default!;
    public int LongestStreak { get; set; }
}

internal class GetSummaryStatsQueryHandler(
   ICurrentUserService currentUserService,
   IHabitRepository habitRepository,
   IHabitLogRepository habitLogRepository,
   IUnitOfWork unitOfWork
   ) : IRequestHandler<GetSummaryStatsQuery, Result<GetSummaryStatsQueryResponse>>
{
    public async Task<Result<GetSummaryStatsQueryResponse>> Handle(GetSummaryStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var totalHabits = habitRepository.GetAll()
            .Count(habit => habit.CreateUserId == userId);

        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        var activeHabits = habitLogRepository.GetAll()
            .Where(log => log.CreateUserId == userId && log.Date >= sevenDaysAgo)
            .Select(log => log.HabitId)
            .Distinct()
            .Count();

        var habitLogs = await (from log in habitLogRepository.GetAll()
                               join habit in habitRepository.GetAll()
                               on log.HabitId equals habit.Id
                               where habit.CreateUserId == userId
                               select new HabitInfo
                               {
                                   Id = habit.Id,
                                   Name = habit.Name,
                                   Date = log.Date.Date
                               }).ToListAsync(cancellationToken);

        var result = CalculateLongestStreakAsync(habitLogs);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GetSummaryStatsQueryResponse>.Success(new GetSummaryStatsQueryResponse
        {
            TotalHabits = totalHabits,
            ActiveHabits = activeHabits,
            LongestStreakHabitName = result.habitName,
            LongestStreak = result.streak
        }, "Summary stats retrieved successfully.");
    }

    private class HabitInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime Date { get; set; }
    }

    private (string habitName, int streak) CalculateLongestStreakAsync(List<HabitInfo> habitLogs)
    {
        if (!habitLogs.Any()) return (string.Empty, 0);

        var habitGroups = habitLogs.GroupBy(log => log.Id);
        int longestStreak = 0;
        string longestStreakHabitName = string.Empty;

        foreach (var group in habitGroups)
        {
            var dates = group.Select(g => g.Date).Distinct().OrderBy(d => d).ToList();
            if (!dates.Any()) continue;

            int currentStreak = 1;
            int maxStreakForHabit = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - dates[i - 1]).Days == 1)
                {
                    currentStreak++;
                    maxStreakForHabit = Math.Max(maxStreakForHabit, currentStreak);
                }
                else
                {
                    currentStreak = 1;
                }
            }

            if (maxStreakForHabit > longestStreak)
            {
                longestStreak = maxStreakForHabit;
                longestStreakHabitName = group.First().Name;
            }
        }

        return (longestStreakHabitName, longestStreak);
    }
}


