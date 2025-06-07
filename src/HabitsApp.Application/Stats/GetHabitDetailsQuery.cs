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

namespace HabitsApp.Application.Stats;
public record GetHabitDetailsQuery() : IRequest<Result<List<GetHabitDetailsQueryResponse>>>;

public sealed record GetHabitDetailsQueryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Color { get; set; } = default!;
    public bool IsCompletedToday { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public int TotalCompletions { get; set; }
}

internal sealed class GetHabitDetailsQueryHandler(
    IHabitRepository habitRepository,
    IHabitLogRepository habitLogRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork
    ) : IRequestHandler<GetHabitDetailsQuery, Result<List<GetHabitDetailsQueryResponse>>>
{
    public async Task<Result<List<GetHabitDetailsQueryResponse>>> Handle(GetHabitDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var habits = habitRepository.GetAll()
    .Where(h => h.CreateUserId == userId)
    .Select(h => new { h.Id, h.Name, h.Color })
    .ToList();

        var logs = habitLogRepository.GetAll()
            .Where(log => log.CreateUserId == userId)
            .ToList();


        var response = habits.Select(habit =>
        {
            var habitLogs = logs.Where(log => log.HabitId == habit.Id).Select(l => l.Date).Distinct().OrderBy(date => date).ToList();

            var isCompletedToday = habitLogs.Any(log => log.Date.Date == DateTime.UtcNow.Date);
            var currentStreak = CalculateCurrentStreak(habitLogs);
            var bestStreak = CalculateBestStreak(habitLogs);
            var totalCompletions = habitLogs.Count;

            return new GetHabitDetailsQueryResponse
            {
                Id = habit.Id,
                Name = habit.Name,
                Color = habit.Color,
                IsCompletedToday = isCompletedToday,
                CurrentStreak = currentStreak,
                BestStreak = bestStreak,
                TotalCompletions = totalCompletions
            };
        }).ToList();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<List<GetHabitDetailsQueryResponse>>.Success(response, "Habit details retrieved successfully.");
    }

    private int CalculateBestStreak(List<DateTime> habitLogs)
    {

        if (!habitLogs.Any()) return 0;

        int bestStreak = 0;
        int currentStreak = 1;
        for (int i = 1; i < habitLogs.Count; i++)
        {
            if ((habitLogs[i] - habitLogs[i - 1]).Days == 1)
            {
                currentStreak++;
            }
            else
            {
                bestStreak = Math.Max(bestStreak, currentStreak);
                currentStreak = 1; // Reset current streak

            }
        }
        bestStreak = Math.Max(bestStreak, currentStreak);
        return bestStreak;

    }

    private int CalculateCurrentStreak(List<DateTime> habitLogs)
    {

        if (!habitLogs.Any()) return 0;

        int currentStreak = 1;

        for (int i = habitLogs.Count - 1; i > 0; i--)
        {
            if ((habitLogs[i] - habitLogs[i - 1]).TotalDays == 1)
            {
                currentStreak++;
            }
            else
            {

                break; // Streak broken
            }
        }
        return currentStreak;

    }
}