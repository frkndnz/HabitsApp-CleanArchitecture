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

namespace HabitsApp.Application.Habits;
public sealed record GetHabitByIdQuery(Guid Id):IRequest<Result<GetHabitByIdQueryResponse>>;

public sealed class GetHabitByIdQueryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string Color { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public double CompletionRate { get; set; } = default!;
    public int DaysTracked { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public List<DailyLogStatus>? DailyLogs { get; set; }
    public List<HabitProgressPoint>? Progress { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DailyLogStatus
{
    public DateTime Date { get; set; }
    public bool IsCompleted { get; set; }
}
public class HabitProgressPoint
{
    public DateTime Date { get; set; }
    public int TotalLogsUntilNow { get; set; }
}

internal sealed class GetHabitByIdQueryHandler(
    IHabitRepository habitRepository,
    IHabitLogRepository habitLogRepository,
    ICategoryRepository categoryRepository,
    ICurrentUserService currentUserService
    ) : IRequestHandler<GetHabitByIdQuery, Result<GetHabitByIdQueryResponse>>
{
    public async Task<Result<GetHabitByIdQueryResponse>> Handle(GetHabitByIdQuery request, CancellationToken cancellationToken)
    {
        bool exist=await habitRepository.AnyAsync(h=>h.Id==request.Id && currentUserService.UserId==h.CreateUserId);
        if (!exist)
            return Result<GetHabitByIdQueryResponse>.Failure("not found habits by id");

        var habit=await habitRepository.FirstOrDefaultAsync(h=>h.Id == request.Id);

        var category=await categoryRepository.FirstOrDefaultAsync(c=>c.Id==habit!.CategoryId);

        var habitLogs =  habitLogRepository.GetAll().Where(l => l.HabitId == habit!.Id && currentUserService.UserId == l.CreateUserId).AsEnumerable();
        var habitLogsOnlyDate = habitLogs.Select(l => l.Date).Distinct().OrderBy(date => date).ToList();


        var daysTracked=habitLogs.Select(log=>log.Date.Date).Distinct().Count();
        var startDate = habit!.CreatedAt.Date;
        var totalDays= (DateTime.UtcNow.Date-startDate.Date).Days+1;
        var CompletionRate =totalDays>0 ?
            Math.Round((double) daysTracked / totalDays*100,2)
            :0;
        var bestStreak = CalculateBestStreak(habitLogsOnlyDate);
        var currentStreak=CalculateCurrentStreak(habitLogsOnlyDate);
        var dailyLogs = CalculateDailyLog(habitLogsOnlyDate);
        var progressList = CalculateProgress(habitLogsOnlyDate,startDate);

        var response = new GetHabitByIdQueryResponse
        {
            Id = habit!.Id,
            Name = habit!.Name,
            Description = habit!.Description,
            Color = habit!.Color,
            CategoryId = category?.Id,
            CategoryName = category?.Name,
            CompletionRate = CompletionRate,
            DailyLogs = dailyLogs,
            Progress = progressList,
            BestStreak = bestStreak,
            CurrentStreak = currentStreak,
            DaysTracked = daysTracked,
            CreatedAt= startDate,
        };

        return Result<GetHabitByIdQueryResponse>.Success(response,"successfully");
    }
    private List<HabitProgressPoint> CalculateProgress(List<DateTime> habitLogs,DateTime creatAt)
    {
        var progressList= new List<HabitProgressPoint>();
        var normalizedLogDates = habitLogs
        .Select(x => x.Date)
        .Distinct()
        .ToHashSet();

        int cumulative = 0;
        DateTime progressStart = creatAt.Date;
        DateTime today = DateTime.UtcNow.Date;
       
        for(var date=progressStart;date <=today;date=date.AddDays(1))
        {
            if (normalizedLogDates.Contains(date))
                cumulative++;

            progressList.Add(new HabitProgressPoint
            {
                Date = date,
                TotalLogsUntilNow = cumulative,
            });
        }
        return progressList;
    }
    private List<DailyLogStatus> CalculateDailyLog(List<DateTime> habitLogs)
    {
        var normalizedLogDates = habitLogs
        .Select(x => x.Date)
        .Distinct()
        .ToHashSet();

        var dailyLogs=Enumerable.Range(0,30)
            .Select(i =>
            {
                var date = DateTime.UtcNow.Date.AddDays(-i);
                return new DailyLogStatus
                {
                    Date = date,
                    IsCompleted = normalizedLogDates.Contains(date),
                };
            })
            .OrderBy(dl=>dl.Date)
            .ToList();

        return dailyLogs;
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
